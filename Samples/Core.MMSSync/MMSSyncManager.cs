using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.MMSSync
{
    
    public class MMSSyncManager
    {
        #region Public methods
        /// <summary>
        /// Copies a term group from one term store to another
        /// </summary>
        /// <param name="sourceContext">ClientContext of the source SharePoint site</param>
        /// <param name="targetContext">ClientContext of the target SharePoint site</param>
        /// <param name="termGroupExclusions">Optionally specify a list of groups that will not be copied</param>
        /// <param name="termGroupToCopy">Optionally specify which termgroup should be copied</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool CopyNewTermGroups(ClientContext sourceContext, ClientContext targetContext, List<string> termGroupExclusions = null, string termGroupToCopy = null)
        {
            TermStore sourceTermStore = GetTermStoreObject(sourceContext);
            TermStore targetTermStore = GetTermStoreObject(targetContext);

            // Do both source and target have a common language...if not no sync will happen. 
            // A list of common languages will be outputted: only content for these languages will be copied, no additional languages are
            // added to the target termstore as this might imply deployment of additional language packs
            List<int> languagesToProcess = null;
            if (!ValidTermStoreLanguages(sourceTermStore, targetTermStore, out languagesToProcess))
            {
                Log.Internal.TraceError((int)EventId.LanguageMismatch, "The target termstore default language is not available as language in the source term store, syncing cannot proceed.");
                return false;
            }

            // Get a list of termgroups to process, exclude site collection scoped groups and system groups
            IEnumerable<TermGroup> termGroups = sourceContext.LoadQuery(sourceTermStore.Groups.Include(g => g.Name,
                                                                                                       g => g.Id,
                                                                                                       g => g.IsSiteCollectionGroup,
                                                                                                       g => g.IsSystemGroup))
                                                                                              .Where(g => g.IsSystemGroup == false && g.IsSiteCollectionGroup == false);
            sourceContext.ExecuteQuery();

            foreach (TermGroup termGroup in termGroups)
            {
                // skip term group if we're only interested in copying one particular term group
                if (!String.IsNullOrEmpty(termGroupToCopy))
                {
                    if (!termGroup.Name.Equals(termGroupToCopy, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                // Skip term groups that we do not want to copy
                if (termGroupExclusions != null && termGroupExclusions.Contains(termGroup.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    Log.Internal.TraceInformation((int)EventId.CopyTermGroup_Skip, "Skipping {0} as this is a system termgroup", termGroup.Name);
                    continue;
                }

                // About to start copying of a term group
                TermGroup sourceTermGroup = GetTermGroup(sourceContext, sourceTermStore, termGroup.Name);
                TermGroup targetTermGroup = GetTermGroup(targetContext, targetTermStore, termGroup.Name);

                if (sourceTermGroup == null)
                {
                    continue;
                }
                if (targetTermGroup != null)
                {
                    if (sourceTermGroup.Id != targetTermGroup.Id)
                    {
                        //Term group exists with a different ID...can't sync
                        Log.Internal.TraceWarning((int)EventId.CopyTermGroup_IDMismatch, "The term groups have different ID's. I don't know how to work it.");
                    }
                    else
                    {
                        // do nothing as this termgroup was previously copied. Termgroup changes need to be 
                        // picked up by the changelog processing
                        Log.Internal.TraceInformation((int)EventId.CopyTermGroup_AlreadyCopied, "Termgroup {0} was already copied...changes to it will need to come from changelog processing.", termGroup.Name);
                    }
                }
                else
                {
                    Log.Internal.TraceInformation((int)EventId.CopyTermGroup_Copying, "Copying termgroup {0}...", termGroup.Name);
                    this.CreateNewTargetTermGroup(sourceContext, targetContext, sourceTermGroup, targetTermStore, languagesToProcess);
                }
            }

            return true;
        }

        /// <summary>
        /// Copy a particular term group between term stores
        /// </summary>
        /// <param name="sourceContext">ClientContext of the source SharePoint site</param>
        /// <param name="targetContext">ClientContext of the target SharePoint site</param>
        /// <param name="termGroup">Specify which termgroup should be copied</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool CopyTermGroup(ClientContext sourceContext, ClientContext targetContext, string termGroup)
        {
            return this.CopyNewTermGroups(sourceContext, targetContext, null, termGroup);
        }

        /// <summary>
        /// Processes the change log from the source term store and applies the changes to the target termstore
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of the source SharePoint site</param>
        /// <param name="targetClientContext">ClientContext of the target SharePoint site</param>
        /// <param name="startFrom">DateTime object indicating from which date/time we would like to process the changelog</param>
        /// <param name="termGroupExclusions">Optional parameter that shows the termgroups that are excluded from processing</param>
        /// <param name="termSetInclusions">Optional parameter that shows the termsets that exclusively processed, others are skipped if this is defined</param>
        /// <returns>True if successful, false otherwise</returns>       
        public bool ProcessChanges(ClientContext sourceClientContext, ClientContext targetClientContext, DateTime startFrom, List<string> termGroupExclusions = null, List<string> termSetInclusions = null)
        {
            Log.Internal.TraceInformation((int)EventId.TaxonomySession_Open, "Opening the taxonomy session");
            TaxonomySession sourceTaxonomySession = TaxonomySession.GetTaxonomySession(sourceClientContext);
            TermStore sourceTermStore = sourceTaxonomySession.GetDefaultKeywordsTermStore();
            sourceClientContext.Load(sourceTermStore,
                                            store => store.Name,
                                            store => store.DefaultLanguage,
                                            store => store.Languages,
                                            store => store.Groups.Include(group => group.Name, group => group.Id));
            sourceClientContext.ExecuteQuery();

            Log.Internal.TraceInformation((int)EventId.TermStore_GetChangeLog, "Reading the changes");
            ChangeInformation changeInformation = new ChangeInformation(sourceClientContext);
            changeInformation.StartTime = startFrom;
            ChangedItemCollection termStoreChanges = sourceTermStore.GetChanges(changeInformation);
            sourceClientContext.Load(termStoreChanges);
            sourceClientContext.ExecuteQuery();

            if (termStoreChanges.Count > 0)
            {
                Log.Internal.TraceInformation((int)EventId.TermStore_NumberOfChanges, "Number of changes returned: {0}", termStoreChanges.Count);

                bool noError = true;
                // Load up the taxonomy item names.
                TaxonomySession targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                TermStore targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                targetClientContext.Load(targetTermStore,
                                            store => store.Name,
                                            store => store.DefaultLanguage,
                                            store => store.Languages,
                                            store => store.Groups.Include(group => group.Name, group => group.Id));
                targetClientContext.ExecuteQuery();

                List<int> languagesToProcess = null;
                if (!ValidTermStoreLanguages(sourceTermStore, targetTermStore, out languagesToProcess))
                {
                    Log.Internal.TraceError((int)EventId.LanguageMismatch, "The target termstore default language {0} is not available as language in the source term store, syncing cannot proceed.");
                    return false;
                }

                foreach (ChangedItem _changeItem in termStoreChanges)
                {
                    //sometimes we get stale entries, code should handle them but for performance reasons it's better to filter them out
                    if (_changeItem.ChangedTime < startFrom)
                    {
                        Log.Internal.TraceVerbose((int)EventId.TermStore_SkipChangeLogEntry, "Skipping item {1} changed at {0}", _changeItem.ChangedTime, _changeItem.Id);
                        continue;
                    }

                    Log.Internal.TraceVerbose((int)EventId.TermStore_ProcessChangeLogEntry, "Processing item {1} changed at {0}. Operation = {2}, ItemType = {3}", _changeItem.ChangedTime, _changeItem.Id, _changeItem.Operation, _changeItem.ItemType);

                    #region Group changes
                    if (_changeItem.ItemType == ChangedItemType.Group)
                    {
                        #region Delete group
                        if (_changeItem.Operation == ChangedOperationType.DeleteObject)
                        {
                            TermGroup targetTermGroup = targetTermStore.GetGroup(_changeItem.Id);
                            targetClientContext.Load(targetTermGroup, group => group.Name);
                            targetClientContext.ExecuteQuery();

                            if (!targetTermGroup.ServerObjectIsNull.Value)
                            {
                                if (termGroupExclusions == null || !termGroupExclusions.Contains(targetTermGroup.Name, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermGroup_Delete, "Deleting group: {0}", targetTermGroup.Name);
                                    targetTermGroup.DeleteObject();
                                    targetClientContext.ExecuteQuery();
                                }
                            }
                        }
                        #endregion
                        else
                        {
                            TermGroup sourceTermGroup = sourceTermStore.GetGroup(_changeItem.Id);
                            sourceClientContext.Load(sourceTermGroup, group => group.Name,
                                                                      group => group.Id,
                                                                      group => group.IsSystemGroup,
                                                                      group => group.Description);
                            sourceClientContext.ExecuteQuery();
                            if (sourceTermGroup.ServerObjectIsNull.Value)
                            {
                                //source group not found...can happen is SharePoint is sending stale entries in the changelog (or when too old entries are requested)
                                continue;
                            }
                            else
                            {
                                if (sourceTermGroup.IsSystemGroup)
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermGroup_IsSystemGroup, "Group {0} is a system group", sourceTermGroup.Name);
                                    continue;
                                }
                            }

                            #region Add group
                            if (_changeItem.Operation == ChangedOperationType.Add)
                            {
                                TermGroup targetTermGroup = targetTermStore.GetGroup(_changeItem.Id);
                                targetClientContext.Load(targetTermGroup, group => group.Name,
                                                                          group => group.Id);

                                targetClientContext.ExecuteQuery();

                                // If group already exists no action is needed
                                if (targetTermGroup.ServerObjectIsNull.Value)
                                {
                                    TermGroup targetTermGroupTest = targetTermStore.Groups.GetByName(sourceTermGroup.Name);
                                    targetClientContext.Load(targetTermGroupTest, group => group.Name);

                                    try
                                    {
                                        targetClientContext.ExecuteQuery();
                                        if (!targetTermGroupTest.ServerObjectIsNull.Value)
                                        {
                                            if (sourceTermGroup.Name.ToLower() == "system" || sourceTermGroup.Name.ToLower() == "people")
                                            {
                                                Log.Internal.TraceInformation((int)EventId.TermGroup_AlreadyExists, "Group {0} already exists", sourceTermGroup.Name);
                                                continue;
                                            }
                                            else
                                            {
                                                InvalidOperationException uEx = new InvalidOperationException(String.Format("A group named {0} already exists but with a different ID. Please delete the term group from the target termstore", sourceTermGroup.Name));
                                                Log.Internal.TraceError((int)EventId.TermGroup_IDMismatch, uEx, "A group named {0} already exists but with a different ID. Please delete the term group from the target termstore", sourceTermGroup.Name);
                                                break;
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    if (termGroupExclusions == null || !termGroupExclusions.Contains(sourceTermGroup.Name, StringComparer.InvariantCultureIgnoreCase))
                                    {
                                        Log.Internal.TraceInformation((int)EventId.TermGroup_Add, "Adding group {0}", sourceTermGroup.Name);
                                        TermGroup _targetTermGroup = targetTermStore.CreateGroup(sourceTermGroup.Name, _changeItem.Id);
                                        if (!string.IsNullOrEmpty(sourceTermGroup.Description))
                                        {
                                            _targetTermGroup.Description = sourceTermGroup.Description;
                                        }
                                        targetClientContext.ExecuteQuery();
                                    }
                                }
                            }
                            #endregion
                            #region Edit group
                            else if (_changeItem.Operation == ChangedOperationType.Edit)
                            {
                                TermGroup targetTermGroup = targetTermStore.GetGroup(_changeItem.Id);
                                targetClientContext.Load(targetTermGroup, group => group.Name,
                                                                          group => group.Id);

                                targetClientContext.ExecuteQuery();
                                if (targetTermGroup.ServerObjectIsNull.Value)
                                {
                                    targetTermGroup = targetTermStore.Groups.GetByName(sourceTermGroup.Name);

                                    targetClientContext.Load(targetTermGroup, group => group.Name);
                                    targetClientContext.ExecuteQuery();
                                    if (targetTermGroup.ServerObjectIsNull.Value)
                                    {
                                        noError = false;
                                        break;
                                    }
                                }

                                if (termGroupExclusions == null || !termGroupExclusions.Contains(sourceTermGroup.Name, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermGroup_Edit, "Modifying group: {0}", sourceTermGroup.Name);
                                    targetTermGroup.Name = sourceTermGroup.Name;
                                    if (!string.IsNullOrEmpty(sourceTermGroup.Description))
                                    {
                                        targetTermGroup.Description = sourceTermGroup.Description;
                                    }
                                    targetClientContext.ExecuteQuery();
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region TermSet changes
                    if (_changeItem.ItemType == ChangedItemType.TermSet)
                    {
                        #region Delete termset
                        if (_changeItem.Operation == ChangedOperationType.DeleteObject)
                        {
                            TermSet targetTermset = targetTermStore.GetTermSet(_changeItem.Id);
                            targetClientContext.Load(targetTermset);
                            targetClientContext.ExecuteQuery();

                            if (!targetTermset.ServerObjectIsNull.Value)
                            {
                                //Check if the termset is in the group we're processing. If the termset inclusion list is not set then this will just return true
                                if (IsTermSetInIncludedTermSets(targetClientContext, targetTermset, termSetInclusions))
                                {
                                    if (IsTermSetInExcludedGroup(targetClientContext, targetTermset, termGroupExclusions))
                                    {
                                        Log.Internal.TraceInformation((int)EventId.TermSet_Skip, "Skip termset {0} due to included in excluded group", targetTermset.Name);
                                        continue;
                                    }
                                }
                                else
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermSet_Skip_Inclusion, "Skip termset {0} due to not included in termset inclusion group", targetTermset.Name);
                                    continue;
                                }

                                Log.Internal.TraceInformation((int)EventId.TermSet_Delete, "Deleting termset {0}", targetTermset.Name);

                                targetTermset.DeleteObject();
                                targetClientContext.ExecuteQuery();
                            }
                        }
                        #endregion
                        else
                        {
                            TermSet sourceTermset = sourceTermStore.GetTermSet(_changeItem.Id);
                            sourceClientContext.Load(sourceTermset, termset => termset.Name,
                                                                    termset => termset.Id,
                                                                    termset => termset.Description,
                                                                    termset => termset.Contact,
                                                                    termset => termset.CustomSortOrder,
                                                                    termset => termset.CustomProperties,
                                                                    termset => termset.IsAvailableForTagging,
                                                                    termset => termset.IsOpenForTermCreation,
                                                                    termset => termset.CustomProperties,
                                                                    termset => termset.Terms, terms => terms.Id, terms => terms.Name, terms => terms.Description,
                                                                    termset => termset.Group, group => group.Id);
                            sourceClientContext.ExecuteQuery();

                            if (sourceTermset.ServerObjectIsNull.Value)
                            {
                                continue;
                            }
                            else
                            {
                                //Check if the termset is in the group we're processing. If the termset inclusion list is not set then this will just return true
                                if (IsTermSetInIncludedTermSets(sourceClientContext, sourceTermset, termSetInclusions))
                                {
                                    //Check if the termset is not in a group we're not processing
                                    if (IsTermSetInExcludedGroup(sourceClientContext, sourceTermset, termGroupExclusions))
                                    {
                                        Log.Internal.TraceInformation((int)EventId.TermSet_Skip, "Skip termset {0} due to included in excluded group", sourceTermset.Name);
                                        continue;
                                    }
                                }
                                else
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermSet_Skip_Inclusion, "Skip termset {0} due to not included in termset inclusion group", sourceTermset.Name);
                                    continue;
                                }
                            }

                            #region Move termset
                            if (_changeItem.Operation == ChangedOperationType.Move)
                            {
                                TermSet targetTermSetCheck = targetTermStore.GetTermSet(_changeItem.Id);
                                targetClientContext.Load(targetTermSetCheck, termset => termset.Name,
                                                                             termset => termset.Group, group => group.Id);

                                try
                                {
                                    targetClientContext.ExecuteQuery();
                                    if (!targetTermSetCheck.ServerObjectIsNull.Value)
                                    {
                                        //find the group of the source termset
                                        TermGroup _targetTermGroup = targetTermStore.GetGroup(sourceTermset.Group.Id);
                                        targetClientContext.Load(_targetTermGroup, group => group.Id,
                                                                                   group => group.Name);
                                        targetClientContext.ExecuteQuery();

                                        if (!_targetTermGroup.ServerObjectIsNull.Value)
                                        {
                                            if (!targetTermSetCheck.Group.Id.Equals(_targetTermGroup.Id))
                                            {
                                                //move the termset to this group
                                                Log.Internal.TraceInformation((int)EventId.TermSet_Move, "Move termset {0} to group {1}", targetTermSetCheck.Name, _targetTermGroup.Name);
                                                targetTermSetCheck.Move(_targetTermGroup);
                                                targetClientContext.ExecuteQuery();
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            #endregion
                            #region Copy termset
                            else if (_changeItem.Operation == ChangedOperationType.Copy)
                            {
                                TermGroup targetTermGroup = targetTermStore.GetGroup(sourceTermset.Group.Id);
                                targetClientContext.Load(targetTermGroup, group => group.Name);
                                targetClientContext.ExecuteQuery();

                                TermSet targetTermSetCheck = targetTermStore.GetTermSet(sourceTermset.Id);
                                targetClientContext.Load(targetTermSetCheck);
                                try
                                {
                                    targetClientContext.ExecuteQuery();
                                    if (!targetTermSetCheck.ServerObjectIsNull.Value)
                                    {
                                        Log.Internal.TraceInformation((int)EventId.TermSet_AlreadyExists, "Termset: {0} already exists", sourceTermset.Name);
                                        continue;
                                    }
                                }
                                catch
                                {
                                }

                                Log.Internal.TraceInformation((int)EventId.TermSet_Copy, "Copying termset: {0}", sourceTermset.Name);
                                TermSet targetTermSet = targetTermGroup.CreateTermSet(sourceTermset.Name, _changeItem.Id, targetTermStore.DefaultLanguage);

                                //Refresh session and termstore references to force reload of the termset just added. This is 
                                //needed cause there can be a update change event following next and without this trick
                                //the newly created termset cannot be obtained from the server
                                targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                                targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                                targetClientContext.Load(targetTermStore, store => store.Name,
                                                                          store => store.DefaultLanguage,
                                                                          store => store.Languages,
                                                                          store => store.Groups.Include(group => group.Name, group => group.Id));
                                targetClientContext.Load(targetTermSet, termset => termset.CustomProperties);
                                targetClientContext.ExecuteQuery();

                                UpdateTermSet(sourceClientContext, targetClientContext, sourceTermset, targetTermSet);

                                // reuse first level of children with the recursive flag set to true
                                foreach (Term child in sourceTermset.Terms)
                                {
                                    Term targetChild = targetTermStore.GetTerm(child.Id);
                                    targetClientContext.Load(targetChild);
                                    targetClientContext.ExecuteQuery();

                                    if (!targetChild.ServerObjectIsNull.Value)
                                    {
                                        targetTermSet.ReuseTerm(targetChild, true);
                                    }
                                }
                                targetClientContext.ExecuteQuery();
                            }
                            #endregion
                            #region Add termset
                            else if (_changeItem.Operation == ChangedOperationType.Add)
                            {
                                TermGroup targetTermGroup = targetTermStore.GetGroup(sourceTermset.Group.Id);
                                targetClientContext.Load(targetTermGroup, group => group.Name);
                                targetClientContext.ExecuteQuery();

                                if (targetTermGroup.ServerObjectIsNull.Value)
                                {
                                    //Group may exist with another name
                                    targetTermGroup = targetTermStore.Groups.GetByName(sourceTermset.Group.Name);
                                    targetClientContext.Load(targetTermGroup, group => group.Name);
                                    targetClientContext.ExecuteQuery();
                                    if (targetTermGroup.ServerObjectIsNull.Value)
                                    {
                                        noError = false;
                                        break;
                                    }
                                }

                                TermSet targetTermSetCheck = targetTermGroup.TermSets.GetByName(sourceTermset.Name);
                                targetClientContext.Load(targetTermSetCheck);

                                try
                                {
                                    targetClientContext.ExecuteQuery();
                                    if (!targetTermSetCheck.ServerObjectIsNull.Value)
                                    {
                                        Log.Internal.TraceInformation((int)EventId.TermSet_AlreadyExists, "Termset: {0} already exists", sourceTermset.Name);
                                        continue;
                                    }
                                }
                                catch
                                {
                                }

                                Log.Internal.TraceInformation((int)EventId.TermSet_Add, "Adding termset: {0}", sourceTermset.Name);
                                TermSet _targetTermSet = targetTermGroup.CreateTermSet(sourceTermset.Name, _changeItem.Id, targetTermStore.DefaultLanguage);

                                //Refresh session and termstore references to force reload of the term just added. This is 
                                //needed cause there can be a update change event following next and without this trick
                                //the newly created termset cannot be obtained from the server
                                targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                                targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                                targetClientContext.Load(targetTermStore, store => store.Name,
                                                                          store => store.DefaultLanguage,
                                                                          store => store.Languages,
                                                                          store => store.Groups.Include(group => group.Name, group => group.Id));
                                targetClientContext.Load(_targetTermSet, termset => termset.CustomProperties);
                                targetClientContext.ExecuteQuery();

                                UpdateTermSet(sourceClientContext, targetClientContext, sourceTermset, _targetTermSet);
                            }
                            #endregion
                            #region Edit termset
                            else if (_changeItem.Operation == ChangedOperationType.Edit)
                            {
                                TermGroup targetTermGroup = null;
                                TermSet targetTermSet = targetTermStore.GetTermSet(_changeItem.Id);
                                targetClientContext.Load(targetTermSet, termset => termset.Name,
                                                                        termset => termset.CustomProperties);
                                targetClientContext.ExecuteQuery();

                                if (targetTermSet.ServerObjectIsNull.Value)
                                {
                                    targetTermGroup = targetTermStore.GetGroup(sourceTermset.Group.Id);
                                    targetClientContext.Load(targetTermGroup, group => group.Name);
                                    targetClientContext.ExecuteQuery();
                                    if (!targetTermGroup.ServerObjectIsNull.Value)
                                    {
                                        targetTermSet = targetTermGroup.TermSets.GetByName(sourceTermset.Name);
                                        targetClientContext.Load(targetTermSet, termset => termset.Name,
                                                                                termset => termset.CustomProperties);
                                        targetClientContext.ExecuteQuery();
                                    }
                                }

                                if (!targetTermSet.ServerObjectIsNull.Value)
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermSet_Edit, "Modifying termset: {0}", sourceTermset.Name);
                                    UpdateTermSet(sourceClientContext, targetClientContext, sourceTermset, targetTermSet);
                                }
                                else
                                {
                                    Log.Internal.TraceInformation((int)EventId.TermSet_NotFoundCreating, "Termset: {0} not found, creating it", sourceTermset.Name);
                                    TermSet _targetTermSet = targetTermGroup.CreateTermSet(sourceTermset.Name, _changeItem.Id, targetTermStore.DefaultLanguage);

                                    //Refresh session and termstore references to force reload of the termset just added. This is 
                                    //needed cause there can be a update change event following next and without this trick
                                    //the newly created termset cannot be obtained from the server
                                    targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                                    targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                                    targetClientContext.Load(targetTermStore, store => store.Name,
                                                                              store => store.DefaultLanguage,
                                                                              store => store.Languages,
                                                                              store => store.Groups.Include(group => group.Name, group => group.Id));
                                    targetClientContext.Load(targetTermSet, termset => termset.Name,
                                                                            termset => termset.CustomProperties);
                                    targetClientContext.ExecuteQuery();

                                    UpdateTermSet(sourceClientContext, targetClientContext, sourceTermset, _targetTermSet);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region Term changes
                    if (_changeItem.ItemType == ChangedItemType.Term)
                    {
                        #region Delete term
                        if (_changeItem.Operation == ChangedOperationType.DeleteObject)
                        {
                            Term targetTerm = targetTermStore.GetTerm(_changeItem.Id);
                            targetClientContext.Load(targetTerm);
                            targetClientContext.ExecuteQuery();

                            if (!targetTerm.ServerObjectIsNull.Value)
                            {
                                //Check if the termset is in the group we're processing. If the termset inclusion list is not set then this will just return true
                                if (IsTermInIncludedTermSets(targetClientContext, targetTerm, termSetInclusions))
                                {
                                    if (IsTermInExcludedGroup(targetClientContext, targetTerm, termGroupExclusions))
                                    {
                                        Log.Internal.TraceInformation((int)EventId.Term_Skip, "Skip term {0} due to included in excluded group", targetTerm.Name);
                                        continue;
                                    }
                                }
                                else
                                {
                                    Log.Internal.TraceInformation((int)EventId.Term_Skip_Inclusion, "Skip term {0} due to not included in termset inclusion group", targetTerm.Name);
                                    continue;
                                }

                                Log.Internal.TraceInformation((int)EventId.Term_Delete, "Deleting term: {0}", targetTerm.Name);
                                targetTerm.DeleteObject();
                                targetClientContext.ExecuteQuery();
                            }
                        }
                        #endregion
                        else
                        {
                            Term sourceTerm = sourceTermStore.GetTerm(_changeItem.Id);
                            sourceClientContext.Load(sourceTerm, term => term.Name,
                                                                 term => term.Description,
                                                                 term => term.Id,
                                                                 term => term.Parent,
                                                                 term => term.IsAvailableForTagging,
                                                                 term => term.IsDeprecated,
                                                                 term => term.CustomSortOrder,
                                                                 term => term.LocalCustomProperties,
                                                                 term => term.CustomProperties,
                                                                 term => term.Labels.Include(label => label.Value, label => label.Language, label => label.IsDefaultForLanguage),
                                                                 term => term.TermSet, termset => termset.Id);
                            sourceClientContext.ExecuteQuery();

                            if (!sourceTerm.ServerObjectIsNull.Value)
                            {
                                //Check if the termset is in the group we're processing. If the termset inclusion list is not set then this will just return true
                                if (IsTermInIncludedTermSets(sourceClientContext, sourceTerm, termSetInclusions))
                                {
                                    //Check if the termset is not in a group we're not processing
                                    if (IsTermInExcludedGroup(sourceClientContext, sourceTerm, termGroupExclusions))
                                    {
                                        Log.Internal.TraceInformation((int)EventId.Term_Skip, "Skip term {0} due to included in excluded group", sourceTerm.Name);
                                        continue;
                                    }
                                }
                                else
                                {
                                    Log.Internal.TraceInformation((int)EventId.Term_Skip_Inclusion, "Skip term {0} due to not included in termset inclusion group", sourceTerm.Name);
                                    continue;
                                }

                                TermSet sourceTermSet = sourceTermStore.GetTermSet(sourceTerm.TermSet.Id);
                                sourceClientContext.Load(sourceTermSet, termset => termset.Name,
                                                                        termset => termset.Id,
                                                                        termset => termset.Group);
                                sourceClientContext.ExecuteQuery();

                                TermSet targetTermSet = targetTermStore.GetTermSet(sourceTerm.TermSet.Id);
                                targetClientContext.Load(targetTermSet, termset => termset.Name);
                                targetClientContext.ExecuteQuery();

                                if (targetTermSet.ServerObjectIsNull.Value)
                                {
                                    noError = false;
                                    break;
                                }

                                #region Move term
                                else if (_changeItem.Operation == ChangedOperationType.Move)
                                {
                                    Term targetTerm = targetTermStore.GetTerm(_changeItem.Id);
                                    targetClientContext.Load(targetTerm);
                                    targetClientContext.ExecuteQuery();

                                    if (!targetTerm.ServerObjectIsNull.Value)
                                    {

                                        TermSetItem newParent = null;
                                        if (!sourceTerm.Parent.ServerObjectIsNull.Value)
                                        {
                                            // Source was moved to another term
                                            sourceClientContext.Load(sourceTerm, term => term.Parent, parent => parent.Id);
                                            sourceClientContext.ExecuteQuery();
                                            newParent = targetTermStore.GetTerm(sourceTerm.Parent.Id);
                                        }
                                        else
                                        {
                                            // Source was moved the root of a termset
                                            sourceClientContext.Load(sourceTerm, term => term.TermSet, termset => termset.Id);
                                            sourceClientContext.ExecuteQuery();
                                            newParent = targetTermStore.GetTermSet(sourceTerm.TermSet.Id);
                                        }

                                        if (newParent != null)
                                        {
                                            Log.Internal.TraceInformation((int)EventId.Term_Move, "Term {0} moved to parent id {1}", targetTerm.Name, sourceTerm.TermSet.Id);
                                            targetTerm.Move(newParent);
                                            targetClientContext.ExecuteQuery();
                                        }
                                    }
                                }
                                #endregion
                                #region Copy term
                                else if (_changeItem.Operation == ChangedOperationType.Copy)
                                {
                                    //when a term is copied with it's children you'll see multiple copy changelog entries
                                    Term targetTerm = targetTermStore.GetTerm(_changeItem.Id);
                                    targetClientContext.Load(targetTerm);
                                    targetClientContext.ExecuteQuery();

                                    if (targetTerm.ServerObjectIsNull.Value)
                                    {
                                        Term _targetTerm = null;
                                        // Check if the term is a sub term
                                        if (!sourceTerm.Parent.ServerObjectIsNull.Value)
                                        {
                                            sourceClientContext.Load(sourceTerm, term => term.Parent, parent => parent.Id);
                                            sourceClientContext.ExecuteQuery();

                                            //find the parent target term with the same ID
                                            Term parentTargetTerm = targetTermStore.GetTerm(sourceTerm.Parent.Id);
                                            targetClientContext.Load(parentTargetTerm);
                                            targetClientContext.ExecuteQuery();

                                            _targetTerm = parentTargetTerm.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);
                                        }
                                        else
                                        {
                                            _targetTerm = targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);
                                        }

                                        //Refresh session and termstore references to force reload of the term just added. This is 
                                        //needed cause there can be a update change event following next and without this trick
                                        //the newly created term cannot be obtained from the server
                                        targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                                        targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                                        targetClientContext.Load(targetTermStore, store => store.Name,
                                                                                  store => store.DefaultLanguage,
                                                                                  store => store.Languages,
                                                                                  store => store.Groups.Include(group => group.Name, group => group.Id));
                                        targetClientContext.Load(_targetTerm, term => term.IsDeprecated,
                                                                              term => term.LocalCustomProperties,
                                                                              term => term.CustomProperties);
                                        targetClientContext.ExecuteQuery();

                                        UpdateTerm(sourceClientContext, targetClientContext, sourceTerm, _targetTerm, languagesToProcess);

                                        targetClientContext.ExecuteQuery();
                                        Log.Internal.TraceInformation((int)EventId.Term_Copy, "Term {0} copied", _targetTerm.Name);
                                    }
                                }
                                #endregion
                                #region PathChange
                                else if (_changeItem.Operation == ChangedOperationType.PathChange)
                                {
                                    //PathChange Term detected...can be ignored so it seems
                                }
                                #endregion
                                #region Merge term
                                else if (_changeItem.Operation == ChangedOperationType.Merge)
                                {
                                    Term targetTerm = targetTermStore.GetTerm(sourceTerm.Id);
                                    targetClientContext.Load(targetTerm);
                                    targetClientContext.ExecuteQuery();

                                    if (!targetTerm.ServerObjectIsNull.Value)
                                    {
                                        sourceClientContext.Load(sourceTerm, term => term.MergedTermIds);
                                        sourceClientContext.ExecuteQuery();

                                        foreach (Guid mergedTermId in sourceTerm.MergedTermIds)
                                        {
                                            Term termToMerge = targetTermStore.GetTerm(mergedTermId);
                                            targetClientContext.Load(termToMerge, term => term.Name);
                                            targetClientContext.ExecuteQuery();

                                            if (!termToMerge.ServerObjectIsNull.Value)
                                            {
                                                if (!targetTerm.MergedTermIds.Contains<Guid>(mergedTermId))
                                                {
                                                    termToMerge.Merge(targetTerm);
                                                    targetClientContext.ExecuteQuery();
                                                    Log.Internal.TraceInformation((int)EventId.Term_Merge, "Term {0} merged to term {1}", termToMerge.Name, targetTerm.Name);
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Add Term
                                else if (_changeItem.Operation == ChangedOperationType.Add)
                                {
                                    Term targetTerm = targetTermStore.GetTerm(sourceTerm.Id);
                                    targetClientContext.Load(targetTerm);
                                    targetClientContext.ExecuteQuery();

                                    if (targetTerm.ServerObjectIsNull.Value)
                                    {
                                        Log.Internal.TraceInformation((int)EventId.Term_Add, "Creating term: {0}", sourceTerm.Name);

                                        Term _targetTerm = null;

                                        // Check if the term is a sub term
                                        if (!sourceTerm.Parent.ServerObjectIsNull.Value)
                                        {
                                            sourceClientContext.Load(sourceTerm, term => term.Parent, parent => parent.Id);
                                            sourceClientContext.ExecuteQuery();

                                            //find the parent target term with the same ID
                                            Term parentTargetTerm = targetTermStore.GetTerm(sourceTerm.Parent.Id);
                                            targetClientContext.Load(parentTargetTerm);
                                            targetClientContext.ExecuteQuery();

                                            _targetTerm = parentTargetTerm.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);
                                        }
                                        else
                                        {
                                            _targetTerm = targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);
                                        }

                                        //Refresh session and termstore references to force reload of the term just added. This is 
                                        //needed cause there can be a update change event following next and without this trick
                                        //the newly created term cannot be obtained from the server
                                        targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                                        targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                                        targetClientContext.Load(targetTermStore, store => store.Name,
                                                                                  store => store.DefaultLanguage,
                                                                                  store => store.Languages,
                                                                                  store => store.Groups.Include(group => group.Name, group => group.Id));
                                        targetClientContext.Load(_targetTerm, term => term.IsDeprecated,
                                                                              term => term.LocalCustomProperties,
                                                                              term => term.CustomProperties);
                                        targetClientContext.ExecuteQuery();

                                        UpdateTerm(sourceClientContext, targetClientContext, sourceTerm, _targetTerm, languagesToProcess);
                                    }
                                }
                                #endregion
                                #region Edit term
                                else if (_changeItem.Operation == ChangedOperationType.Edit)
                                {
                                    Term targetTerm = targetTermStore.GetTerm(_changeItem.Id);
                                    targetClientContext.Load(targetTerm, term => term.Name,
                                                                         term => term.Description,
                                                                         term => term.IsAvailableForTagging,
                                                                         term => term.IsDeprecated,
                                                                         term => term.LocalCustomProperties,
                                                                         term => term.CustomProperties,
                                                                         term => term.Labels.Include(label => label.Value, label => label.Language, label => label.IsDefaultForLanguage));
                                    targetClientContext.ExecuteQuery();

                                    if (!targetTerm.ServerObjectIsNull.Value)
                                    {
                                        UpdateTerm(sourceClientContext, targetClientContext, sourceTerm, targetTerm, languagesToProcess);
                                        Log.Internal.TraceInformation((int)EventId.Term_Edit, "Term {0} updated", targetTerm.Name);
                                    }
                                    else
                                    {

                                        try
                                        {
                                            Term _targetTerm = targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);

                                            //Refresh session and termstore references to force reload of the term just added. This is 
                                            //needed cause there can be a update change event following next and without this trick
                                            //the newly created termset cannot be obtained from the server
                                            targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                                            targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                                            targetClientContext.Load(targetTermStore, store => store.Name,
                                                                                      store => store.DefaultLanguage,
                                                                                      store => store.Languages,
                                                                                      store => store.Groups.Include(group => group.Name, group => group.Id));
                                            targetClientContext.Load(_targetTerm, term => term.IsDeprecated,
                                                                                  term => term.LocalCustomProperties,
                                                                                  term => term.CustomProperties);
                                            targetClientContext.ExecuteQuery();

                                            UpdateTerm(sourceClientContext, targetClientContext, sourceTerm, _targetTerm, languagesToProcess);
                                            Log.Internal.TraceInformation((int)EventId.Term_NotFoundCreating, "Term: {0} not found, creating it", sourceTerm.Name);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                if (noError)
                {
                    targetClientContext.ExecuteQuery();
                }
            }

            return true;
        }
        #endregion

        #region Private helper methods
        private void CreateNewTargetTermGroup(ClientContext sourceClientContext, ClientContext targetClientContext, TermGroup sourceTermGroup, TermStore targetTermStore, List<int> languagesToProcess)
        {
            TermGroup destinationTermGroup = targetTermStore.CreateGroup(sourceTermGroup.Name, sourceTermGroup.Id);
            if (!string.IsNullOrEmpty(sourceTermGroup.Description))
            {
                destinationTermGroup.Description = sourceTermGroup.Description;
            }

            TermSetCollection sourceTermSetCollection = sourceTermGroup.TermSets;
            if (sourceTermSetCollection.Count > 0)
            {
                foreach (TermSet sourceTermSet in sourceTermSetCollection)
                {
                    sourceClientContext.Load(sourceTermSet,
                                              set => set.Name,
                                              set => set.Description,
                                              set => set.Id,
                                              set => set.Contact,
                                              set => set.CustomProperties,
                                              set => set.IsAvailableForTagging,
                                              set => set.IsOpenForTermCreation,
                                              set => set.CustomSortOrder,
                                              set => set.CustomProperties,
                                              set => set.Terms.Include(
                                                        term => term.Name,
                                                        term => term.Description,
                                                        term => term.Id,
                                                        term => term.IsAvailableForTagging,
                                                        term => term.LocalCustomProperties,
                                                        term => term.CustomProperties,
                                                        term => term.IsDeprecated,
                                                        term => term.Labels.Include(label => label.Value, label => label.Language, label => label.IsDefaultForLanguage)));

                    sourceClientContext.ExecuteQuery();

                    TermSet targetTermSet = destinationTermGroup.CreateTermSet(sourceTermSet.Name, sourceTermSet.Id, targetTermStore.DefaultLanguage);
                    targetClientContext.Load(targetTermSet, set => set.CustomProperties);
                    targetClientContext.ExecuteQuery();
                    UpdateTermSet(sourceClientContext, targetClientContext, sourceTermSet, targetTermSet);

                    foreach (Term sourceTerm in sourceTermSet.Terms)
                    {
                        Term reusedTerm = targetTermStore.GetTerm(sourceTerm.Id);
                        targetClientContext.Load(reusedTerm);
                        targetClientContext.ExecuteQuery();

                        Term targetTerm;
                        if (reusedTerm.ServerObjectIsNull.Value)
                        {
                            try
                            {
                                targetTerm = targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, sourceTerm.Id);
                                targetClientContext.Load(targetTerm, term => term.IsDeprecated,
                                                                     term => term.CustomProperties,
                                                                     term => term.LocalCustomProperties);
                                targetClientContext.ExecuteQuery();
                                UpdateTerm(sourceClientContext, targetClientContext, sourceTerm, targetTerm, languagesToProcess);
                            }
                            catch (ServerException ex)
                            {
                                if (ex.Message.IndexOf("Failed to read from or write to database. Refresh and try again.") > -1)
                                {
                                    // This exception was due to caching issues and generally is thrown when there's term reuse accross groups
                                    targetTerm = targetTermSet.ReuseTerm(reusedTerm, false);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                        }
                        else
                        {
                            targetTerm = targetTermSet.ReuseTerm(reusedTerm, false);
                        }

                        targetClientContext.Load(targetTerm);
                        targetClientContext.ExecuteQuery();

                        targetTermStore.UpdateCache();

                        //Refresh session and termstore references to force reload of the term just added. This is 
                        //needed cause there can be a update change event following next and without this trick
                        //the newly created termset cannot be obtained from the server
                        targetTermStore = GetTermStoreObject(targetClientContext);

                        //recursively add the other terms
                        ProcessSubTerms(sourceClientContext, targetClientContext, targetTermSet, targetTerm, sourceTerm, languagesToProcess, targetTermStore.DefaultLanguage);
                    }
                }
            }
            targetClientContext.ExecuteQuery();
        }

        private void ProcessSubTerms(ClientContext sourceClientContext, ClientContext targetClientContext, TermSet targetTermSet, Term targetTerm, Term sourceTerm, List<int> languagesToProcess, int defaultLanguage)
        {
            TermCollection sourceTerms = sourceTerm.Terms;
            sourceClientContext.Load(sourceTerms, terms => terms.Include(
                                                              term => term.Name,
                                                              term => term.Description,
                                                              term => term.Id,
                                                              term => term.IsDeprecated,
                                                              term => term.IsAvailableForTagging,
                                                              term => term.LocalCustomProperties,
                                                              term => term.CustomProperties,
                                                              term => term.CustomSortOrder,
                                                              term => term.Labels.Include(label => label.Value, label => label.Language, label => label.IsDefaultForLanguage),
                                                              term => term.TermSet, termset => termset.Id));
            sourceClientContext.ExecuteQuery();
            foreach (Term subSourceTerm in sourceTerm.Terms)
            {
                Term reusedTerm = targetTerm.TermStore.GetTerm(subSourceTerm.Id);
                targetClientContext.Load(reusedTerm);
                targetClientContext.ExecuteQuery();

                Term childTargetTerm;
                if (reusedTerm.ServerObjectIsNull.Value)
                {

                    childTargetTerm = targetTerm.CreateTerm(subSourceTerm.Name, defaultLanguage, subSourceTerm.Id);
                    targetClientContext.Load(childTargetTerm, term => term.IsDeprecated,
                                                              term => term.CustomProperties,
                                                              term => term.LocalCustomProperties);
                    targetClientContext.ExecuteQuery();
                    UpdateTerm(sourceClientContext, targetClientContext, subSourceTerm, childTargetTerm, languagesToProcess);
                }
                else
                {
                    childTargetTerm = targetTerm.ReuseTerm(reusedTerm, false);
                }

                targetClientContext.Load(childTargetTerm);
                targetClientContext.ExecuteQuery();

                targetTermSet.TermStore.UpdateCache();

                ProcessSubTerms(sourceClientContext, targetClientContext, targetTermSet, childTargetTerm, subSourceTerm, languagesToProcess, defaultLanguage);
            }
        }

        private TermStore GetTermStoreObject(ClientContext clientContext)
        {
            TaxonomySession sourceTaxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            TermStore sourceTermStore = sourceTaxonomySession.GetDefaultKeywordsTermStore();
            clientContext.Load(sourceTermStore,
                                store => store.Name,
                                store => store.DefaultLanguage,
                                store => store.Languages);
            clientContext.ExecuteQuery();

            return sourceTermStore;
        }

        private TermGroup GetTermGroup(ClientContext clientContext, TermStore termStore, string groupName)
        {
            TermGroup termGroup = termStore.Groups.GetByName(groupName);
            clientContext.Load(termGroup, group => group.Name,
                                          group => group.Id,
                                          group => group.Description,
                                          group => group.TermSets.Include(
                                                   termSet => termSet.Name,
                                                   termSet => termSet.Id));
            try
            {
                clientContext.ExecuteQuery();
                if (!termGroup.ServerObjectIsNull.Value)
                {
                    return termGroup;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private bool ValidTermStoreLanguages(TermStore sourceTermStore, TermStore targetTermStore, out List<int> languagesToProcess)
        {
            //check if the default target termstore language is availble as source termstore language
            //build a list of languages to process
            bool defaultLanguageFound = false;
            languagesToProcess = new List<int>();
            foreach (int sourceLanguage in sourceTermStore.Languages)
            {
                foreach (int targetLanguage in targetTermStore.Languages)
                {
                    if (targetLanguage == sourceLanguage)
                    {
                        languagesToProcess.Add(targetLanguage);
                    }

                    if (targetTermStore.DefaultLanguage == sourceLanguage)
                    {
                        defaultLanguageFound = true;
                    }
                }
            }

            if (!defaultLanguageFound)
            {
                return false;
            }

            return true;
        }

        private bool IsTermSetInExcludedGroup(ClientContext clientContext, TermSet termSet, List<string> termGroupExclusions)
        {
            if (termGroupExclusions == null)
            {
                return false;
            }

            clientContext.Load(termSet, ts => ts.Group);
            clientContext.Load(termSet.Group, group => group.Name);
            clientContext.ExecuteQuery();

            if (!termGroupExclusions.Contains(termSet.Group.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private bool IsTermSetInIncludedTermSets(ClientContext clientContext, TermSet termSet, List<string> termSetInclusions)
        {
            if (termSetInclusions == null)
            {
                return true;
            }

            clientContext.Load(termSet, ts => ts.Name);
            clientContext.ExecuteQuery();

            if (termSetInclusions.Contains(termSet.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private bool IsTermInExcludedGroup(ClientContext clientContext, Term term, List<string> termGroupExclusions)
        {
            if (termGroupExclusions == null)
            {
                return false;
            }

            clientContext.Load(term, t => t.TermSet);
            clientContext.Load(term.TermSet, ts => ts.Group);
            clientContext.Load(term.TermSet.Group, group => group.Name);
            clientContext.ExecuteQuery();

            if (!termGroupExclusions.Contains(term.TermSet.Group.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private bool IsTermInIncludedTermSets(ClientContext clientContext, Term term, List<string> termSetInclusions)
        {
            if (termSetInclusions == null)
            {
                return true;
            }

            clientContext.Load(term, t => t.TermSet);
            clientContext.Load(term.TermSet, ts => ts.Name);
            clientContext.ExecuteQuery();

            if (termSetInclusions.Contains(term.TermSet.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private void UpdateTermSet(ClientContext sourceClientContext, ClientContext targetClientContext, TermSet sourceTermset, TermSet targetTermSet)
        {
            targetTermSet.Description = sourceTermset.Description;
            targetTermSet.IsAvailableForTagging = sourceTermset.IsAvailableForTagging;
            targetTermSet.Contact = sourceTermset.Contact;
            targetTermSet.CustomSortOrder = sourceTermset.CustomSortOrder;
            // We're not allowing term creation as the target termset should stay in sync with the source
            targetTermSet.IsOpenForTermCreation = false;

            // Copy termset properties
            if (sourceTermset.CustomProperties.Count > 0)
            {
                // add properties from source
                foreach (KeyValuePair<string, string> property in sourceTermset.CustomProperties)
                {
                    targetTermSet.SetCustomProperty(property.Key, property.Value);
                }
            }

            if (targetTermSet.CustomProperties.Count > 0)
            {
                //remove properties which are not needed anymore
                foreach (KeyValuePair<string, string> property in targetTermSet.CustomProperties)
                {
                    if (!sourceTermset.CustomProperties.Keys.Contains(property.Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        targetTermSet.DeleteCustomProperty(property.Key);
                    }
                }
            }

            targetClientContext.ExecuteQuery();
        }

        private void UpdateTerm(ClientContext sourceClientContext, ClientContext targetClientContext, Term sourceTerm, Term targetTerm, List<int> languagesToProcess)
        {
            targetTerm.Name = sourceTerm.Name;
            targetTerm.IsAvailableForTagging = sourceTerm.IsAvailableForTagging;
            targetTerm.CustomSortOrder = sourceTerm.CustomSortOrder;

            // Process custom properties
            if (sourceTerm.LocalCustomProperties.Count > 0)
            {
                // add properties from source
                foreach (KeyValuePair<string, string> property in sourceTerm.LocalCustomProperties)
                {
                    targetTerm.SetLocalCustomProperty(property.Key, property.Value);
                }
            }

            if (targetTerm.LocalCustomProperties.Count > 0)
            {
                //remove properties which are not needed anymore
                foreach (KeyValuePair<string, string> property in targetTerm.LocalCustomProperties)
                {
                    if (!sourceTerm.LocalCustomProperties.Keys.Contains(property.Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        targetTerm.DeleteLocalCustomProperty(property.Key);
                    }
                }
            }

            if (sourceTerm.CustomProperties.Count > 0)
            {
                // add properties from source
                foreach (KeyValuePair<string, string> property in sourceTerm.CustomProperties)
                {
                    targetTerm.SetCustomProperty(property.Key, property.Value);
                }
            }

            if (targetTerm.CustomProperties.Count > 0)
            {
                //remove properties which are not needed anymore
                foreach (KeyValuePair<string, string> property in targetTerm.CustomProperties)
                {
                    if (!sourceTerm.CustomProperties.Keys.Contains(property.Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        targetTerm.DeleteCustomProperty(property.Key);
                    }
                }
            }

            targetClientContext.ExecuteQuery();

            foreach (int language in languagesToProcess)
            {
                ClientResult<string> targetTermDescription = sourceTerm.GetDescription(language);
                sourceClientContext.ExecuteQuery();
                targetTerm.SetDescription(targetTermDescription.Value, language);

                //Process labels
                // Add new labels if needed
                LabelCollection sourceLabels = sourceTerm.GetAllLabels(language);
                sourceClientContext.Load(sourceLabels);
                sourceClientContext.ExecuteQuery();
                LabelCollection targetLabels = targetTerm.GetAllLabels(language);
                targetClientContext.Load(targetLabels);
                targetClientContext.ExecuteQuery();
                foreach (Label sourceLabel in sourceLabels)
                {
                    Label test = targetLabels.FirstOrDefault<Label>(label => label.Value.Equals(sourceLabel.Value));

                    if (test == null)
                    {
                        targetTerm.CreateLabel(sourceLabel.Value, sourceLabel.Language, sourceLabel.IsDefaultForLanguage);
                    }
                }
                targetClientContext.ExecuteQuery();

                // remove non existing labels
                targetLabels = targetTerm.GetAllLabels(language);
                targetClientContext.Load(targetLabels);
                targetClientContext.ExecuteQuery();
                List<Label> labelsToRemove = new List<Label>();
                foreach (Label targetLabel in targetLabels)
                {
                    Label test = sourceLabels.FirstOrDefault<Label>(label => label.Value.Equals(targetLabel.Value));
                    if (test == null)
                    {
                        labelsToRemove.Add(targetLabel);
                    }
                }

                foreach (Label label in labelsToRemove)
                {
                    label.DeleteObject();
                    targetClientContext.ExecuteQuery();
                }
            }

            // Deprecating terms on the target is tricky...this will prevent reuse of deprecited terms later on
            //if (sourceTerm.IsDeprecated && !targetTerm.IsDeprecated)
            //{
            //    targetTerm.Deprecate(true);
            //    targetClientContext.ExecuteQuery();
            //}
            //else if (!sourceTerm.IsDeprecated && targetTerm.IsDeprecated)
            //{
            //    targetTerm.Deprecate(false);
            //    targetClientContext.ExecuteQuery();
            //}

        }
        #endregion                
    }

}

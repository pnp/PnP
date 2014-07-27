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
        private List<TermOperation> _termStoreOperations = new List<TermOperation>();
        private TermStore sourceTermStore = null;
        private TermStore targetTermStore = null;
        private TermGroup sourceTermGroup = null;
        private TermGroup targetTermGroup = null;
        private TermGroup _destinationTermGroup = null;

        public List<TermOperation> Logs
        {
            get
            {
                return this._termStoreOperations;
            }

        }
        public void MoveTermGroup(ClientContext sourceCtx, ClientContext targetCtx, string termGroup)
        {
          
            this._termStoreOperations = new List<TermOperation>();

            this.sourceTermStore = this.GetTermStoreObject(sourceCtx);
            this.targetTermStore = this.GetTermStoreObject(targetCtx);

            this.sourceTermGroup = this.GetTermGroup(sourceCtx, sourceTermStore, termGroup);
            this.targetTermGroup = this.GetTermGroup(targetCtx, targetTermStore, termGroup);

            if (sourceTermGroup == null)
            {
                return;
            }
            if (targetTermGroup != null)
            {
                if (sourceTermGroup.Id != targetTermGroup.Id)
                {
                    throw new Exception("The Term groups have different ID's. I don't know how to work it.");
                }
            }
            else
            {
                this.CreateTargetNewTermGroup(sourceCtx, targetCtx, sourceTermGroup, targetTermStore);
            }
        }

        private TermStore GetTermStoreObject(ClientContext ctx)
        {
            TaxonomySession _sourceTaxonomySession = TaxonomySession.GetTaxonomySession(ctx);
            TermStore _sourceTermStore = _sourceTaxonomySession.GetDefaultKeywordsTermStore();
            ctx.Load(_sourceTermStore,
                    store => store.Name,
                    store => store.DefaultLanguage,
                    store => store.Groups.Include(
                        group => group.Name, group => group.Id, group => group.Description,
                        group => group.TermSets.Include(
                            termSet => termSet.Name, termSet => termSet.Id,
                            termSet => termSet.Terms.Include(
                                term => term.Name, term => term.Id)
                        )
                    )
            );
            ctx.ExecuteQuery();

            return _sourceTermStore;
        }

        private TermGroup GetTermGroup(ClientContext ctx, TermStore termStore, string groupName)
        {
            TermGroup _termGroup = termStore.Groups.GetByName(groupName);

            ctx.Load(_termGroup, group => group.Name, group => group.Id, group => group.Description,
                group => group.TermSets.Include(
                        termSet => termSet.Name,
                        termSet => termSet.Id));

            try
            {
                ctx.ExecuteQuery();
                if(!_termGroup.ServerObjectIsNull.Value)
                {
                    return _termGroup;
                }
                else
                {
                    return null;
                }

            }
            catch(Exception _ex)
            {
                return null;
            }
        }

        private void CreateTargetNewTermGroup(ClientContext sourceClientContext, ClientContext targetClientContext, TermGroup sourceTermGroup, TermStore targetTermStore)
        {
            try
            {
                this._destinationTermGroup = targetTermStore.CreateGroup(sourceTermGroup.Name, sourceTermGroup.Id);
                if(!string.IsNullOrEmpty(sourceTermGroup.Description))
                {
                    this._destinationTermGroup.Description = sourceTermGroup.Description;
                }
                TermOperation _op = new TermOperation();
                _op.Term = sourceTermGroup.Name;
                _op.Id = sourceTermGroup.Id.ToString();
                _op.Operation = "Add";
                _op.Type = "TermGroup";
                this._termStoreOperations.Add(_op);
     
                TermSetCollection _sourceTermSetCollection = sourceTermGroup.TermSets;
                if (_sourceTermSetCollection.Count > 0)
                {
                    foreach (TermSet _sourceTermSet in _sourceTermSetCollection)
                    {
                        sourceClientContext.Load(_sourceTermSet,
                                                  set => set.Name,
                                                  set => set.Description,
                                                  set => set.Id,
                                                  set => set.Terms.Include(
                                                            term => term.Name,
                                                            term => term.Id),
                                                            term => term.Description,
                                                            term => term.Contact);
                                                   

                        sourceClientContext.ExecuteQuery();
                        
                        TermSet _targetTermSet = _destinationTermGroup.CreateTermSet(_sourceTermSet.Name, _sourceTermSet.Id, targetTermStore.DefaultLanguage);
                        if(!string.IsNullOrEmpty(_sourceTermSet.Description))
                        {
                             _targetTermSet.Description = _sourceTermSet.Description;
                        }
                        foreach(Term _sourceTerm in _sourceTermSet.Terms)
                        {
                             Term _targetTerm = _targetTermSet.CreateTerm(_sourceTerm.Name, targetTermStore.DefaultLanguage, _sourceTerm.Id);
                             _op = new TermOperation();
                             _op.Term = _sourceTerm.Name;
                             _op.Id = _sourceTerm.Id.ToString();
                             _op.Operation = "Add";
                             _op.Type = "Term";
                             this._termStoreOperations.Add(_op);
                        }
                    }

                }
                try
                {
                    targetClientContext.ExecuteQuery();
                    targetTermStore.CommitAll();
                }
                catch
                {
                    throw;
                }
               
            }
            catch
            {
                throw;
            }
        }

        public void ProcessChanges(ClientContext sourceClientContext, ClientContext targetClientContext , List<string> termSetNames)
        {   
            List<TermOperation> _list = new List<TermOperation>();
            DateTime _startFrom = DateTime.Now.AddYears(-1);

            Console.WriteLine("Opening the taxonomy session");
            TaxonomySession _sourceTaxonomySession = TaxonomySession.GetTaxonomySession(sourceClientContext);
            TermStore sourceTermStore = _sourceTaxonomySession.GetDefaultKeywordsTermStore();
            sourceClientContext.Load(sourceTermStore);
            sourceClientContext.ExecuteQuery();

            Console.WriteLine("Reading the changes");
            ChangeInformation _ci = new ChangeInformation(sourceClientContext);
            _ci.StartTime = _startFrom;
            ChangedItemCollection _cic = sourceTermStore.GetChanges(_ci);
            sourceClientContext.Load(_cic);
            sourceClientContext.ExecuteQuery();

            if (_cic.Count > 0)
            {
                bool noError = true;
                // Load up the taxonomy item names.
                TaxonomySession targetTaxonomySession = TaxonomySession.GetTaxonomySession(targetClientContext);
                TermStore targetTermStore = targetTaxonomySession.GetDefaultKeywordsTermStore();
                targetClientContext.Load(targetTermStore,
                    store => store.Name,
                    store => store.DefaultLanguage,
                    store => store.Groups.Include(
                        group => group.Name, group => group.Id));
                targetClientContext.ExecuteQuery();

                foreach (ChangedItem _changeItem in _cic)
                {
                    Guid g = _changeItem.Id;
                    if (_changeItem.ItemType == ChangedItemType.Group)
                    {
                        TermGroup sourceTermGroup = sourceTermStore.GetGroup(_changeItem.Id);
                        sourceClientContext.Load(sourceTermGroup, group => group.Name, group => group.Id, group => group.IsSystemGroup, group => group.Description);
                        sourceClientContext.ExecuteQuery();
                        if (sourceTermGroup.ServerObjectIsNull.Value)
                        {
                            continue;
                        }
                        else
                        {
                            if (sourceTermGroup.IsSystemGroup)
                            {
                                Console.WriteLine("Group \"" + sourceTermGroup.Name + "\" is a system group");
                                continue;
                            }
                        }
                   
                        if (_changeItem.Operation == ChangedOperationType.DeleteObject)
                        {
                            TermGroup targetTermGroup = targetTermStore.GetGroup(_changeItem.Id);
                            targetClientContext.Load(targetTermGroup,
                                group => group.Name,
                                group => group.Id,
                                group => group.TermSets.Include(
                                    termSet => termSet.Name,
                                    termSet => termSet.Id));
                            targetClientContext.ExecuteQuery();

                            foreach (TermSet ts in targetTermGroup.TermSets)
                            {
                                Console.WriteLine("Deleting termset: " + ts.Name);
                                TermOperation op = new TermOperation();
                                op.Term = ts.Name;
                                op.Id = ts.Id.ToString();
                                op.Operation = "Delete";
                                op.Type = "TermSet";
                                _list.Add(op);
                                ts.DeleteObject();
                            }

                            Console.WriteLine("Deleting group: " + sourceTermGroup.Name);
                            targetTermGroup.DeleteObject();
                            TermOperation op2 = new TermOperation();
                            op2.Term = sourceTermGroup.Name;
                            op2.Id = _changeItem.Id.ToString();
                            op2.Operation = "Delete";
                            op2.Type = "TermGroup";
                            _list.Add(op2);

                            targetClientContext.ExecuteQuery();
                        }
                        else if (_changeItem.Operation == ChangedOperationType.Add)
                        {
                            TermGroup targetTermGroup = targetTermStore.GetGroup(_changeItem.Id);
                            targetClientContext.Load(targetTermGroup, group => group.Name,
                                group => group.Id,
                                group => group.TermSets.Include(
                                    termSet => termSet.Name,
                                    termSet => termSet.Id));

                            targetClientContext.ExecuteQuery();

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
                                            Console.WriteLine("Group: " + sourceTermGroup.Name + " already exists");
                                            continue;
                                        }
                                        else
                                        {
                                            InvalidOperationException uEx = new InvalidOperationException("A group named: \"" + sourceTermGroup.Name + "\" already exists but with a different ID. Please delete the term group from the target termstore");
                                            break;
                                        }
                                    }
                                }
                                catch
                                {

                                }

                                Console.WriteLine("Adding group: " + sourceTermGroup.Name);
                                TermGroup _targetTermGroup = targetTermStore.CreateGroup(sourceTermGroup.Name, _changeItem.Id);
                                if(!string.IsNullOrEmpty(sourceTermGroup.Description))
                                { 
                                _targetTermGroup.Description = sourceTermGroup.Description;
                                }
                                targetClientContext.ExecuteQuery();
                                targetTermStore.CommitAll();
                                targetTermStore.RefreshLoad();

                                TermOperation op = new TermOperation();
                                op.Term = sourceTermGroup.Name;
                                op.Id = _changeItem.Id.ToString();
                                op.Operation = "Add";
                                op.Type = "TermGroup";

                                _list.Add(op);
                            }
                        }
                        else if (_changeItem.Operation == ChangedOperationType.Edit)
                        {
                            TermGroup targetTermGroup = targetTermStore.GetGroup(_changeItem.Id);
                            targetClientContext.Load(targetTermGroup,
                                group => group.Name,
                                group => group.Id,
                                group => group.TermSets.Include(
                                    termSet => termSet.Name,
                                    termSet => termSet.Id));

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

                            if (targetTermGroup.Name != sourceTermGroup.Name)
                            {
                                Console.WriteLine("Modifying group: " + sourceTermGroup.Name);

                                targetTermGroup.Name = sourceTermGroup.Name;
                                TermOperation op = new TermOperation();
                                op.Term = sourceTermGroup.Name;
                                op.Id = _changeItem.Id.ToString();
                                op.Operation = "Modify";
                                op.Type = "TermGroup";

                                _list.Add(op);

                                targetClientContext.ExecuteQuery();
                            }
                        }
                    }

                    if (_changeItem.ItemType == ChangedItemType.TermSet)
                    {

                        TermSet sourceTermset = sourceTermStore.GetTermSet(_changeItem.Id);
                        sourceClientContext.Load(sourceTermset, termset => termset.Name,
                            termset => termset.Id,
                            termset => termset.Description,
                            termset => termset.Contact,
                            termset => termset.CustomProperties,
                            termset => termset.Group, group => group.Id);
                        sourceClientContext.ExecuteQuery();
                        if (sourceTermset.ServerObjectIsNull.Value)
                        {
                            continue;
                        }
                        if (!termSetNames.Contains(sourceTermset.Name))
                        {
                            continue;
                        }
                   
                        if (_changeItem.Operation == ChangedOperationType.DeleteObject)
                        {
                            TermSet targetTermset = targetTermStore.GetTermSet(_changeItem.Id);
                            targetClientContext.Load(targetTermset);
                            targetClientContext.ExecuteQuery();

                            Console.WriteLine("Deleting termset: " + targetTermset.Name);

                            targetTermset.DeleteObject();
                            targetClientContext.ExecuteQuery();

                            TermOperation op = new TermOperation();
                            op.Term = targetTermset.Name;
                            op.Id = _changeItem.Id.ToString();
                            op.Operation = "Delete";
                            op.Type = "TermSet";

                            _list.Add(op);

                        }
                        else if (_changeItem.Operation == ChangedOperationType.Add)
                        {
                            TermGroup targetTermGroup = targetTermStore.GetGroup(sourceTermset.Group.Id);
                            targetClientContext.Load(targetTermGroup,
                                group => group.Name,
                                group => group.IsSystemGroup,
                                group => group.TermSets.Include(
                                    termSet => termSet.Name, termSet => termSet.Id));
                            targetClientContext.ExecuteQuery();
                            if (targetTermGroup.ServerObjectIsNull.Value)
                            {
                                //Group may exist with another name
                                targetTermGroup = targetTermStore.Groups.GetByName(sourceTermset.Group.Name);
                                targetClientContext.Load(targetTermGroup,
                                    group => group.Name,
                                    group => group.IsSystemGroup,
                                    group => group.TermSets.Include(
                                        termSet => termSet.Name, termSet => termSet.Id));
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
                                    Console.WriteLine("Termset: " + sourceTermset.Name + " already exists");
                                    continue;
                                }
                            }
                            catch
                            {
                            }
                            Console.WriteLine("Adding termset: " + sourceTermset.Name);
                            targetTermGroup.CreateTermSet(sourceTermset.Name, _changeItem.Id, targetTermStore.DefaultLanguage);
                            TermOperation op = new TermOperation();
                            op.Term = sourceTermset.Name;
                            op.Id = _changeItem.Id.ToString();
                            op.Operation = "Add";
                            op.Type = "TermSet";
                            targetClientContext.ExecuteQuery();
                            targetTermStore.CommitAll();
                            targetTermStore.RefreshLoad();

                            _list.Add(op);

                        }
                        else if (_changeItem.Operation == ChangedOperationType.Edit)
                        {
                            TermGroup targetTermGroup = null;
                            TermSet sourceTermSet = sourceTermStore.GetTermSet(_changeItem.Id);
                            sourceClientContext.Load(sourceTermSet, termset => termset.Name);
                            sourceClientContext.ExecuteQuery();

                            TermSet targetTermSet = targetTermStore.GetTermSet(_changeItem.Id);
                            targetClientContext.Load(targetTermSet, termset => termset.Name);
                            targetClientContext.ExecuteQuery();

                            if (targetTermSet.ServerObjectIsNull.Value)
                            {
                                targetTermGroup = targetTermStore.GetGroup(sourceTermset.Group.Id);
                                targetClientContext.Load(targetTermGroup, group => group.Name, group => group.IsSystemGroup);
                                targetClientContext.ExecuteQuery();
                                if (!targetTermGroup.ServerObjectIsNull.Value)
                                {
                                    targetTermSet = targetTermGroup.TermSets.GetByName(sourceTermSet.Name);
                                    targetClientContext.Load(targetTermSet, termset => termset.Name);
                                    targetClientContext.ExecuteQuery();
                                }
                            }

                            if (!targetTermSet.ServerObjectIsNull.Value)
                            {
                                if (targetTermSet.Name != sourceTermSet.Name)
                                {
                                    Console.WriteLine("Modifying termset: " + sourceTermSet.Name);
                                    targetTermSet.Name = sourceTermSet.Name;
                                    TermOperation op = new TermOperation();
                                    op.Term = sourceTermSet.Name;
                                    op.Id = _changeItem.Id.ToString();
                                    op.Operation = "Modify";
                                    op.Type = "TermSet";

                                    _list.Add(op);

                                }
                            }
                            else
                            {
                                Console.WriteLine("Termset: " + sourceTermset.Name + " not found, creating it");
                                targetTermGroup.CreateTermSet(sourceTermset.Name, _changeItem.Id, targetTermStore.DefaultLanguage);
                                TermOperation op = new TermOperation();
                                op.Term = sourceTermset.Name;
                                op.Id = _changeItem.Id.ToString();
                                op.Operation = "Add";
                                op.Type = "TermSet";

                                _list.Add(op);

                            }
                        }
                    }

                    if (_changeItem.ItemType == ChangedItemType.Term)
                    {
                        
                        Term sourceTerm = sourceTermStore.GetTerm(_changeItem.Id);
                        
                        sourceClientContext.Load(sourceTerm,
                            term => term.Name,
                            term => term.Description,
                            term => term.Id,
                            term => term.TermSet,
                            termset => termset.Id);

                        sourceClientContext.ExecuteQuery();
                        if (!sourceTerm.ServerObjectIsNull.Value)
                        {
                            TermSet sourceTermSet = sourceTermStore.GetTermSet(sourceTerm.TermSet.Id);
                            sourceClientContext.Load(sourceTermSet,
                                termset => termset.Name,
                                termset => termset.Id,
                                termset => termset.Group);
                            sourceClientContext.ExecuteQuery();


                            if (!sourceTermSet.ServerObjectIsNull.Value)
                            {
                                if(!termSetNames.Contains(sourceTermSet.Name))
                                {
                                    continue;
                                }
                            }


                            TermSet targetTermSet = targetTermStore.GetTermSet(sourceTerm.TermSet.Id);
                            targetClientContext.Load(targetTermSet, termset => termset.Name);
                            targetClientContext.ExecuteQuery();
                            if (targetTermSet.ServerObjectIsNull.Value)
                            {
                                noError = false;
                                break;

                            }


                            if (_changeItem.Operation == ChangedOperationType.DeleteObject)
                            {
                                Term targetTerm = targetTermStore.GetTerm(_changeItem.Id);
                                targetClientContext.Load(targetTerm);
                                targetClientContext.ExecuteQuery();

                                Console.WriteLine("Deleting term: " + sourceTerm.Name);

                                targetTerm.DeleteObject();
                                TermOperation op = new TermOperation();
                                op.Term = sourceTerm.Name;
                                op.Id = _changeItem.Id.ToString();
                                op.Operation = "Delete";
                                op.Type = "Term";
                                _list.Add(op);

                            }

                            else if (_changeItem.Operation == ChangedOperationType.Add)
                            {

                                Term targetTerm = targetTermStore.GetTerm(sourceTerm.Id);
                                targetClientContext.Load(targetTerm);
                                targetClientContext.ExecuteQuery();

                                if (targetTerm.ServerObjectIsNull.Value)
                                {
                                    Console.WriteLine("Creating term: " + sourceTerm.Name);

                                    Term _targetTerm =  targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);
                                    if (!string.IsNullOrEmpty(sourceTerm.Description))
                                    {
                                        _targetTerm.SetDescription(sourceTerm.Description, targetTermStore.DefaultLanguage);
                                    }
                                    //GetLabels from Source
                                   


                                    targetClientContext.ExecuteQuery();
                                    TermOperation op = new TermOperation();
                                    op.Term = sourceTerm.Name;
                                    op.Id = _changeItem.Id.ToString();
                                    op.Operation = "Add";
                                    op.Type = "Term";

                                    _list.Add(op);
                                }
                            }
                            else if (_changeItem.Operation == ChangedOperationType.Edit)
                            {
                                Term targetTerm = targetTermStore.GetTerm(_changeItem.Id);
                                targetClientContext.Load(targetTerm, term => term.Name);
                                targetClientContext.ExecuteQuery();

                                if (!targetTerm.ServerObjectIsNull.Value)
                                {
                                    if (targetTerm.Name != sourceTerm.Name)
                                    {
                                        targetTerm.Name = sourceTerm.Name;
                                        TermOperation op = new TermOperation();
                                        op.Term = sourceTerm.Name;
                                        op.Id = _changeItem.Id.ToString();
                                        op.Operation = "Modify";
                                        op.Type = "Term";

                                        _list.Add(op);
                                    }
                                }
                                else
                                {

                                    try
                                    {
                                        Term _targetTerm = targetTermSet.CreateTerm(sourceTerm.Name, targetTermStore.DefaultLanguage, _changeItem.Id);
                                        if (!string.IsNullOrEmpty(sourceTerm.Description))
                                        {
                                            _targetTerm.SetDescription(sourceTerm.Description, targetTermStore.DefaultLanguage);
                                        }
                                        targetClientContext.ExecuteQuery();
                                        Console.WriteLine("Term: " + sourceTerm.Name + " not found, creating it");
                                        TermOperation op = new TermOperation();
                                        op.Term = sourceTerm.Name;
                                        op.Id = _changeItem.Id.ToString();
                                        op.Operation = "Add";
                                        op.Type = "Term";
                                        _list.Add(op);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }

                }
                if (noError)
                {   
                    targetClientContext.ExecuteQuery();
                    targetTermStore.CommitAll();
                  }
            }
        }
    }
}

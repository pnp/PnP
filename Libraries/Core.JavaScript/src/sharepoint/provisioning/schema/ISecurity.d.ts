/// <reference path="iroleassignment.d.ts" />

interface ISecurity {
    BreakRoleInheritance: boolean;
    CopyRoleAssignments: boolean;
    ClearSubscopes: boolean;
    RoleAssignments: Array<IRoleAssignment>;
}

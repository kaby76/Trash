grammar acme;
acmeCompUnit
   : (acmeImportDeclaration)* (acmeSystemDeclaration | acmeFamilyDeclaration | acmeDesignDeclaration)+ EOF
   ;

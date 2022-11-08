# web_rulesets
This repository is intended for Skyhigh Security employees and advanced admin users familiar with the Skyhigh policy code engine.  Each directory within the project is a ruleset built to address a particular use case.  To learn more about the Skyhigh policy engine, visit [our documentation](https://success.myshn.net).

## Importing rulesets
Importing rulesets into your policy will require several steps:
1. Create any objects referenced by the code in the `ROUTINE`.  This may include lists in the **List Catalog** *(Policy > Web Policy > List Catalog)* and/or **Feature Configs** *(Policy> Web Policy > Feature Configuration)*.
2. Add an `INCLUDE` statement in the intended parent ruleset to create the new child ruleset in web policy.
3. Insert the desired code from this repository into your newly created ruleset making sure to verify the following:
    * The name of the `ROUTINE` must match the name specified in the `INCLUDE` statement.
    * Change the names of any lists or feature configurations to match those you created in your tenant.
    * Ensure that the triggers (a.k.a. "cycles") the ruleset is configured to be evaluated on are also configured in all parent and ancestor rulesets.
4. If you are an employee or a partner with access to the UI Template Viewer tool (a.k.a. "Olaf's tool"), then you can optionally add the UI JSON if it is included.
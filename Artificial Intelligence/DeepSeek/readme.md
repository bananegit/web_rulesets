<!--Tests:Start-->
<!--Tests:End-->
### Setup

- In SSE go to Policy -> Web Policy -> Policy
- Click on "New Ruleset" in the top right corner and select "Custom Ruleset"
- Set the Ruleset name to whatever you like e.g. DeepSeek Controls
- Select Criteria:
  URL is "chat.deepseek.com"
  OR
  URL is "api.deepseek.com"
- Click on "Applies to: All" and unselect "Embedded Objects"
- To import a Rule from this ruleset click on the 3-dot Menu of your ruleset and select "Add Custom Rule" - "Via Policy Code"
- Name the rule like the github file you want to import
- Click on "Edit" and copy and paste the code of the rule into the policy code view
- Click on "Save" to save your rule
  
### Features

This ruleset provides rules for the following actions for the DeepSeek Webservice and API:

- Limit prompt Length to x characters by cutting off prompt (Web & API)
- Limit prompt Length by replacing prompt with policy warning when max length is exceeded (Web & API)
- Enforce DeepThink settings (Web)
- Enforce Search settings (Web)
- Block File Uploads (Web)
- Prevent pasting from clipboard (Web)

<br/><br/>


### Customizing Settings:

Rule: Limit prompt length (WebApp)

Adjust variable "maxPromptLength" based on your requirements

<br/><br/>

Rule: Replace prompt (WebApp)

Adjust variable "maxPromptLength" and "policyWarning" based on your requirements

<br/><br/>

Rule: Enforce Deepthink Setting (WebApp)

Set the variable "deepThink" to "TRUE" or "FALSE" based on your requirements

<br/><br/>

Rule: Enforce Search Setting (WebApp)

Set the variable "search" to "TRUE" or "FALSE" based on your requirements

<br/><br/>

Rule: Limit prompt length (API)

Adjust variable "maxPromptLength" based on your requirements

Remove entries for roles for that you do NOT want to limit the prompt length for from the "limitedRoles" variable

<br/><br/>

Rule: Replace prompt (API)

Adjust variable "maxPromptLength" based on your requirements

Remove entries for roles for that you do NOT want to replace the prompt for from the "limitedRoles" variable


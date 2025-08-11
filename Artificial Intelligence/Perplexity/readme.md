<!--Tests:Start-->
| Test case | Result |
| --- | --- |
| Block File Upload (WebApp) | :x: |
| Prevent pasting from clipboard (WebApp) | :white_check_mark: |
| Replace prompt (WebApp) | :x: |
Last update: Mon, 11 Aug 2025 06:46:03 GMT
<!--Tests:End-->
### Setup

- In SSE go to Policy -> Web Policy -> Policy
- Click on "New Ruleset" in the top right corner and select "Custom Ruleset"
- Set the Ruleset name to whatever you like e.g. Perplexity Controls
- Select Criteria:
  URL is "www.perplexity.ai"
  OR
  URL is "api.perplexity.ai"
- Click on "Applies to: All" and unselect "Embedded Objects"
- To import a Rule from this ruleset click on the 3-dot Menu of your ruleset and select "Add Custom Rule" - "Via Policy Code"
- Name the rule like the github file you want to import
- Click on "Edit" and copy and paste the code of the rule into the policy code view
- Click on "Save" to save your rule
  
### Features

This rulesets provides rules for the following actions for the Perplexity Webservice and API:

- Limit prompt Length to x characters by cutting off prompt (Web & API)
- Limit prompt Length by replacing prompt with policy warning when max length is exceeded (Web & API)
- Ensure specific sources (web/scholar/social) are enabled (Web)
- Block specific sources (web/scholar/social) (Web)
- Enforce search mode (auto/pro/reasoning/deep research) (Web)
- Block file upload (Web)
- Block Link Sharing (Web)
- Prevent pasting from Clipboard (Web)
- Block microphone access (Web)

<br/><br/>


### Customizing Settings:

Rule: Limit prompt length (WebApp)

Adjust variable "maxPromptLength" based on your requirements

<br/><br/>

Rule: Ensure selected Sources (WebApp)

Adjust variable "ensured_sources" to contain the sources you want to enforce (available sources are "web", "scholar" and "social"

<br/><br/>

Rule: Block selected Sources (WebApp)

Adjust variable "blocked_sources" to contain the sources you want to block (available sources are "web", "scholar" and "social"

<br/><br/>

Rule: Enforce Search Mode (WebApp)

Set the variable "paramsMode" and "paramsModelPreference" variables based on which mode you want to enforce, possible value combinations are described in comments in the policy code

<br/><br/>

Rule: Limit prompt length (API)

Adjust variable "maxPromptLength" based on your requirements

Remove entries for roles for that you do NOT want to limit the prompt length for from the "limitedRoles" variable

<br/><br/>

Rule: Replace prompt (API)

Adjust variable "maxPromptLength" based on your requirements

Remove entries for roles for that you do NOT want to replace the prompt for from the "limitedRoles" variable

















































































































































































































































































































































































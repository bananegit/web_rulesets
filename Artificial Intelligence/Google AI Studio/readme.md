### Setup

- In SSE go to Policy -> Web Policy -> Policy
- Click on "New Ruleset" in the top right corner and select "Custom Ruleset"
- Set the Ruleset name to whatever you like e.g. Google AI Studio Controls
- Select Criteria:
  URL is "aistudio.google.com"
  OR
  URL is "content.googleapis.com"
  OR
  URL is "docs.google.com/picker"
  OR
  URL is "apis.google.com"
- Click on "Applies to: All" and unselect "Embedded Objects"
- To import a Rule from this ruleset click on the 3-dot Menu of your ruleset and select "Add Custom Rule" - "Via Policy Code"
- Name the rule like the github file you want to import
- Click on "Edit" and copy and paste the code of the rule into the policy code view
- Click on "Save" to save your rule
  
### Features

This ruleset provides rules for the following actions for the Gemini Webservice:

- Block prompts exceeding length limit (client-side) (Web)
- Block File Uploads (Web)
- Block File Uploads from Drive (Web)
- Block Link Sharing (Web)
- Prevent pasting from clipboard (Web)
- Block microphone, camera and screen access (Web)

<br/><br/>


### Customizing Settings:

Rule: Block prompts exceeding length limit (WebApp)

Adjust variable "maxPromptLength" based on your requirements

<br/><br/>




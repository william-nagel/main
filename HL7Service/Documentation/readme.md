
# Coding Test for William Nagel

### I chose POST method for the Parse method for the following reasons
- There are no payload capacity restraints on a POST. Because it uses the RequestBody instead of the URL, which has length limitations.
- Idempotency is not important, because we are not creating, editing, or deleting records in a datastore.
- It keeps the API flexible for future extensions, such as parsing options or additional data processing steps.
- It is more secure


## Architecture
### HL7Service: 
### HL7ServiceTest: 

### HL7Controller.ParseMethod: 
Currently the only controller in the project. It is central code with easy to follow data flow, manages flow.
Responsible for instantiating HL7Engine which is the Text Parsing class the does the heavy lifting, applies parsing logic.


### HL7Engine.ParseDataRow: 
This method is probably the method of interest, at the center of the logic for the Code Challenge


### HL7EngineTest:
Unit test class for HL7Engine class.


### HL7ControllerTest:
Unit test class for HL7Controller class.


### HL7Service.Entities Namespace:
Location for all entity (working and property) classes.


### HL7Service.Enums Namespace:
Location for all enumeration types. Makes them easier to locate, I'm not a fan of having them as part of other classes.
As the codebase grows it is much easier to refactor them off to a Common or Core library.


### HL7Service.Extensions Namespace:
Location for all extension methods used to make the code easier to read and self-documenting.


### HL7Service.Exceptions Namespace:
Location for all strongly typed exceptions.


## Unit Tests
1. Visual Studio - HL7ServiceTest project, which consist of two distinct classes.
	Note: performs extensive unit and functional testing 
	a. HL7ControllerTest
	b. HL7EngineTest
2. Postman - HL7 Workspace
    Note: performs end-to-end and behavioral testing 

## Other Comments
- I wrote this API as if I were tackling the rewrite of the legacy L7 service with a RESTful web version, granted naming may not be optimal.
- I'm sure it is not perfect, but without in-depth discussions with the rest of the team, this is the best I could surmise, especially while trying to adhere to the original goal of the coding excercise.
- I didn't over-complicate the code and number of test cases, at this time by adding the TextQualifiers.SingleQuote and TextQualifiers.None enums.
- All input data is NewLineChar normalized to prevent the exponential test cases necessary by implementing the NewLineChars enums.
- There is an xUnit project, and I implemented TDD where the requirements were outlined and defined in the tests prior to writing the actual code.
- There is a Postman Workspace directory which includes an export of Postman Test Workspace used for final end-to-end unit testing.

- To prevent future developers from having to find info, there is a Documentation directory which should store:
	- all readmes.
	- all research information.
	- any & all specifications documentation referenced during development.
	- test data files
	- sophisticated implementation code should reference documents in this folder by title and page.
	
  You will find the following information that may be helpful
	- OriginalInputData.csv: data extracted from coding exercise WORD doc (used by the unit tests)
	- OriginalOutputData.csv: data extracted from coding exercise WORD doc
	
	- HL7Service_Packages.png: HL7Service Project - NuGet Package Dependencies as PNG 
	- HL7ServiceTest_Packages.png: HL7ServiceTest Project - NuGet Package Dependencies as PNG 
	
	- xUnitTestResults.png: xUnitTest Results screenshot, view the UnitTest results as PNG, HL7ControllerTest & HL7EngineTest
	
	- HL7.postman_collection_v2: Postman Workspace Export v2.1
	- 1_Postman_params.png: Postman: Params tab configuration screenshot
	- 2_Postman_authorization.png: Postman: Authorization top of tab screenshot
	- 3_Postman_authorization.png: Postman: Authorization bottom of tab screenshot
	- 4_Postman_request.png: Postman: Request Input tab screenshot
	- 5_Postman_response.png: Postman: Response Output tab screenshot
	- 6_Postman_GetNewAccessToken.png: Generating New Access Token screenshot 1
	- 7_Postman_GetNewAccessToken.png: Generating New Access Token screenshot 2

	- 1_azure_certs.png: Microsoft Azure: App Certification configuration screenshot
	- 2_azure_perms.png: Microsoft Azure: App Permission configuration screenshot
	- 3_azure_expose.png: Microsoft Azure: App Expose API configuration screenshot
	
- Implemented version capabilities into the RESTful API, to permit side-by-side testing, and permit transitional rollout, and backwards compatibility.
	- v1 folder is implemented and active.
	- v2 folder is included for illustratation purposes only.
	
- Implemented logging which is outputted to the ProcessMonitor Debugger and Application EventViewer with customizable logging levels by editing appsettings.json.
- Implementation of Microsoft Identity services to secure the API



# Project Details ( NuGet and Microsoft ) 
## Client Secret Name
AzureAD:ClientSecret

## Client Secret Value 
_CX8Q~FL4slSW6RLMpReDYWG0OEE8EuusxVfTc.p

G0K8Q~3tyN.~nyKPXoXEK0FzexwDw-OsJiLgGb-l

## Project Output during initial configuration 
Connecting to Microsoft identity platform dependency identityapp1 in the project...
Adding AzureAD:ClientSecret to store LocalSecretsFile...
Updating remote app registration: dotnet msidentity --update-app-registration 
--username nagel.william@gmail.com 
--tenant-id 2518dfa4-aca7-4d13-9094-58ff8149df3d 
--client-id 967717ea-155e-4fae-a552-193095172957 
--enable-id-token --json --instance https://login.microsoftonline.com/ 
--redirect-uris http://localhost:45043/ https://localhost:44334/ https://localhost:7285/

App registration HenrySchein (967717ea-155e-4fae-a552-193095172957) did not require any remote updates
Installing NuGet packages to project...
Installing package 'Microsoft.Identity.Web' with version '2.16.0'.
Skipping package 'Microsoft.Identity.Web', same version or a newer version is already installed.
Installing package 'Microsoft.Identity.Web.UI' with version '2.16.0'.
Skipping package 'Microsoft.Identity.Web.UI', same version or a newer version is already installed.
Installing package 'Microsoft.Identity.Web.DownstreamApi' with version '2.16.0'.
Installing package 'Microsoft.AspNetCore.Authentication.JwtBearer' with version '6.0.14'.
Skipping package 'Microsoft.AspNetCore.Authentication.JwtBearer', same version or a newer version is already installed.
Installing package 'Microsoft.AspNetCore.Authentication.OpenIdConnect' with version '6.0.14'.
Skipping package 'Microsoft.AspNetCore.Authentication.OpenIdConnect', same version or a newer version is already installed.
Installing package 'Microsoft.Identity.Web.MicrosoftGraph' with version '2.16.0'.

Updating project code and settings: dotnet msidentity --update-project 
--username nagel.william@gmail.com 
--tenant-id 2518dfa4-aca7-4d13-9094-58ff8149df3d 
--client-id 967717ea-155e-4fae-a552-193095172957 
--project-file-path "C:\git\HL7Service\HL7Service\HL7Service\HL7Service.csproj" 
--calls-graph=True --calls-downstream-api=True --code-update=true --json 
--instance https://login.microsoftonline.com/

Modified Program.cs
Modified code file Program.cs
Modified code file WeatherForecastController.cs
Serializing new Microsoft identity platform dependency metadata to disk...
SuccessComplete. Microsoft identity platform identityapp1 is configured.

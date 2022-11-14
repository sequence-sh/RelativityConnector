# v0.18.0 (2022-11-14)

## Summary of Changes

- Sequence has a new home: https://gitlab.com/sequence
- The namespace has been updated from `Reductech.Sequence` to `Sequence`

## Issues Closed in this Release

### Other

- Update namespace and paths after move to Sequence group #199

# v0.17.0 (2022-08-29)

- Updated `EntityImportClient` to `0.4.0`
- Updated `Relativity.DataExchange.Client.SDK` to `1.17.7`
- Updated `Relativity.Services.Interfaces.SDK` to `13.6.2`

# v0.16.0 (2022-07-13)

- Enabled [Source Link](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink)
- Enabled publish to [Nuget.org](https://www.nuget.org) including symbols
- Update Core to v0.16.0

# v0.15.0 (2022-05-27)

Maintenance release - dependency updates only.

# v0.14.0 (2022-03-25)

Maintenance release - dependency updates only.

# v0.13.0 (2022-01-16)

EDR is now Sequence. The following has changed:

- The GitLab group has moved to https://gitlab.com/reductech/sequence
- The root namespace is now `Reductech.Sequence`
- The documentation site has moved to https://sequence.sh

Everything else is still the same - automation, simplified.

The project has now been updated to use .NET 6.

## Issues Closed in this Release

### Maintenance

- Rename EDR to Sequence #59
- Update Core to support SCLObject types #55
- Upgrade to use .net 6 #54

# v0.12.0 (2021-11-26)

Maintenance release - dependency updates only.

# v0.11.2 (2021-10-19)

Update `EntityImportClient` to v0.1.1.

## Issues Closed in this Release

### Maintenance

- Update EntityImportClient to v0.1.1 for releases #44

# v0.11.1 (2021-09-17)

Bug fix release.

## Issues Closed in this Release

### Bug Fixes

- Step descriptions are missing from the documentation #41

# v0.11.0 (2021-09-08)

Initial release. Version numbers are aligned with [Core](https://gitlab.com/reductech/edr/core).

The release adds functionality to:

- Create new matters and workspaces
- Ingest data from CSV/Concordance or as entities
- Search and tag items
- Export Data as entities

## Summary of Changes

### Steps

This release adds the following Steps:

- RelativityCreateDynamicObjects
- RelativityCreateField
- RelativityCreateFolder
- RelativityCreateKeywordSearch
- RelativityCreateMatter
- RelativityCreateWorkspace
- RelativityDeleteDocument
- RelativityDeleteKeywordSearch
- RelativityDeleteMatter
- RelativityDeleteUnusedFolders
- RelativityDeleteWorkspace
- RelativityExport
- RelativityGetClients
- RelativityGetMatterStatuses
- RelativityGetSubfolders
- RelativityImport
- RelativityImportEntities
- RelativityMoveFolder
- RelativityReadKeywordSearch
- RelativityRetrieveMatter
- RelativityRetrieveRootFolder
- RelativityRetrieveWorkspace
- RelativityRetrieveWorkspaceStatistics
- RelativitySendQuery
- RelativityUpdateFolder
- RelativityUpdateMatter
- RelativityUpdateObject

## Issues Closed in this Release

### New Features

- Add a CreateField Step to Create Fields #38
- Use a Configurable Relative Path for the Import Client #34
- Create a new Import Client to support a new ImportEntities step #30
- Allow strings to optionally be used instead of ArtifactIds for parameters in Relativity steps to make the Relativity connector easier to use for Technicians #27
- Create Integration tests so that we can be confident the relativity steps work #14
- Create an ImportEntities step so technicians can import entities directly #22
- Create steps that use the KeywordSearchManager so technicians can manipulate searches #23
- Add steps that interact with Relativity Matter Manager so technicians can manipulate matter #21
- Add steps that interact with the relativity Document manager so technicians can manipulate documents #11
- Add a RelativityCreateFolder step, so that a technicians can create new folders #8
- Add a RelativityDeleteWorkspace step, so that a technicians can delete workspaces #13
- Add a RelativityCreateWorkspace step, so that a technicians can create new workspaces #7
- RelativityExport should return Array<Entity> instead of Array<StringStream> #20
- Relativity Export should also optionally export natives #16
- Create a step to export concordance, so that technicians can use sequences with relativity #6
- Create step to import concordance, so that technicians can use sequences with relativity #4
- Rename Template #2

### Bug Fixes

- System.ArrayTypeMismatchException during entity deserialization on EDR #37

### Maintenance

- Update ImportClient submodule and add to connector package #36
- Create a fix for missing or wrong HTTP routes in Relativity manager interfaces #26
- Exclude the Core packages to make sure Relativity works as a Connector #25
- Add support for publishing as a connector #24
- Update Core to latest version #19
- Use template ci config, so that it's easier to maintain #17
- Add Release issue template #18
- Update project readme with any dev programme licensing requirements #5
- Remove epic from issue template #3

### Documentation

- Update the Readme to contain information for the new Steps and Settings #31

### Other

- Add more tests to improve code coverage #35

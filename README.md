# EDR Connector for Relativity®

[Reductech EDR](https://gitlab.com/reductech/edr) is a collection of
libraries that automates cross-application e-discovery and forensic workflows.

The Relativity connector contains Steps to:

- Import/Export data

### [Try Relativity Connector](https://gitlab.com/reductech/edr/edr/-/releases)

Using [EDR](https://gitlab.com/reductech/edr/edr),
the command line tool for running Sequences.

## Documentation

Documentation is available here: https://docs.reductech.io

## E-discovery Reduct

# EDR Relativity Connector

The EDR Relativity Connector allows users to automate forensic workflows using
[Relativity](https://www.relativity.com/).

This connector has [Steps](https://docs.reductech.io/edr/steps/Relativity.html) to:

- Create new matters and workspaces
- Ingest data from CSV/Concordance or as entities
- Search and tag items
- Export Data as entities

[Relativity SCL examples available here](https://docs.reductech.io/edr/examples/relativity.html).

Source code available on [GitLab](https://gitlab.com/reductech/edr/connectors/relativity).

## Relativity Connector Settings

The Relativity Connector requires additional configuration which can be
provided using the `settings` key in `connectors.json`.

### Supported Settings

| Name                  | Required |    Type    |Description                                                                                                                        |
| :- | :-: | :-: | :- |
| RelativityUsername        |    ✔     |  `string`  | The Username to use for authentication |
| RelativityPassword        |    ✔     |  `string`  | The Password to use for authentication |
| Url        |    ✔     |  `string`  | The URL of the Relativity Server |
| DesktopClientPath        |         |  `string`  | The Path to the Relativity Desktop client. Required for the `RelativityImport` step. |
| APIVersionNumber        |         |  `int`  | The version of the API to use. Defaults to `1`. You probably don't need to change this. |

### Example Settings

```json
"Reductech.EDR.Connectors.Nuix": {
  "id": "Reductech.EDR.Connectors.Relativity",
  "enable": true,
  "version": "0.11.0",
  "settings": {
    "RelativityUsername": "YourUsername",
    "RelativityPassword": "YourPassword",
    "Url": "http://relativitydevvm/",
    "DesktopClientPath": "C:\\Program Files\\kCura Corporation\\Relativity Desktop Client\\Relativity.Desktop.Client.exe",
  }
}
```

# Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/edr/connectors/relativity/-/releases).

# NuGet Packages

Are available for download from the [Reductech Nuget feed](https://gitlab.com/reductech/nuget/-/packages).

# Licensing

This product is licensed under the Apache License, Version 2.0.
For further details please see http://www.apache.org/licenses/LICENSE-2.0.

This product may only be used by parties with valid licenses for Relativity®, a product of Relativity ODA LLC.
Relativity ODA LLC does not test, evaluate, endorse or certify this product.

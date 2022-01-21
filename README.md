# Sequence® Connector for Relativity®

The Sequence Connector for Relativity® allows users to automate ediscovery
and forensic workflows that use [Relativity](https://www.relativity.com/).

This connector has [Steps](https://docs.reductech.io/sequence/steps/relativity.html) to:

- Create new matters and workspaces
- Ingest data from CSV/Concordance or as entities
- Search and tag items
- Export Data as entities

## Connector Settings

The Sequence Connector for Relativity® requires additional configuration
which can be provided using the `settings` key in `connectors.json`.

### Supported Settings

| Name               | Required |   Type   | Description                                                                             |
| :----------------- | :------: | :------: | :-------------------------------------------------------------------------------------- |
| RelativityUsername |    ✔     | `string` | The Username to use for authentication                                                  |
| RelativityPassword |    ✔     | `string` | The Password to use for authentication                                                  |
| Url                |    ✔     | `string` | The URL of the Relativity Server                                                        |
| DesktopClientPath  |          | `string` | The Path to the Relativity Desktop client. Required for the `RelativityImport` step.    |
| APIVersionNumber   |          |  `int`   | The version of the API to use. Defaults to `1`. You probably don't need to change this. |

### Example `connectors.json` Entry

```json
"Reductech.Sequence.Connectors.Relativity": {
  "id": "Reductech.Sequence.Connectors.Relativity",
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

## Developing

To successfully run integration tests, a copy of the [EntityImportClient](https://gitlab.com/reductech/sequence/connectors/entityimportclient)
needs to be in the `Sequence.EntityImportClient` directory in the solution root.

Download the latest:

- [Release](https://gitlab.com/reductech/sequence/connectors/entityimportclient/-/releases)
- [Main branch build](https://gitlab.com/reductech/sequence/connectors/entityimportclient/-/jobs/artifacts/main/download?job=package+exe+dev)

The EntityImportClient is automatically downloaded and included in the connector
package when building using the CI.

## Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/sequence/connectors/relativity/-/releases).

## Licensing

This product is licensed under the Apache License, Version 2.0.
For further details please see http://www.apache.org/licenses/LICENSE-2.0.

This product may only be used by parties with valid licenses for Relativity®, a product of Relativity ODA LLC.
Relativity ODA LLC does not test, evaluate, endorse or certify this product.

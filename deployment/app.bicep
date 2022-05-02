param location string = resourceGroup().location
//param storageAccountName string = 'toylaunch${uniqueString(resourceGroup().id)}'
param lastUpdated string = utcNow('u')
param prefix string
param appEnvironment string = 'Dev'
param branch string
param version string

var stackName = '${prefix}${appEnvironment}'

var tags = {
  'stack-name': 'platform'
  'stack-environment': appEnvironment
  'stack-branch': branch
  'stack-version': version
  'stack-last-updated': lastUpdated
  'stack-sub-name': 'demo'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: '${stackName}-jpguerra'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }  
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${stackName}-appInsightsjp'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
  tags:{}
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: '${stackName}-apsjp'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: '${stackName}-function'
  location: location
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
      ]
    }
  }

}



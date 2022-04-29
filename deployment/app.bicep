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

resource site 'Microsoft.Web/sites@2021-03-01' = {
  name: '${stackName}-${uniqueString('site')}'
  location: location
  tags: tags
  properties: {}
}

resource function 'Microsoft.Web/sites/functions@2021-03-01' = {
  name: '${stackName}-${uniqueString('function')}'
  parent: site
  kind: 'web'
  properties: {}
}

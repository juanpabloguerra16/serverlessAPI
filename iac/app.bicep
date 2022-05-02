param location string = resourceGroup().location
//param storageAccountName string = 'toylaunch${uniqueString(resourceGroup().id)}'
param lastUpdated string = utcNow('u')
param prefix string
param appEnvironment string = 'dev'
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

module function './function.bicep' = {
  name: 'deployFunction'
  params: {
    location: location
    stackName: stackName
    tags: tags
  }
}







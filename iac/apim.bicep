param stackName string
param location string
param tags object
param publisherName string = 'BFYOOwner'
param publisherEmail string = 'icecream@bfyo.com'

resource apim 'Microsoft.ApiManagement/service@2021-08-01' = {
  name: '${stackName}-apim'
  location: location
  tags: tags
  sku: {
    capacity: 0
    name: 'Consumption'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
}

resource userAPI 'Microsoft.ApiManagement/service/apis@2021-08-01' = {
  parent: apim
  name: 'user-api'
  properties: {
    subscriptionRequired: true
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    apiRevision: '1'
    isCurrent: true
    displayName: 'User API'
    path: 'user'
    protocols: [
      'http'
      'https'
    ]
  }
}

resource userAPIoperations 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  parent: userAPI
  name:'user-API-operations'
  properties: {
    templateParameters: [
      {
        name: 'userId'
        description: 'User Id'
        type: 'string'
        required: true
        values: []
      }
    ]
    description: 'Use this operation to lookup users.'
    displayName: 'Lookup users'
    method: 'GET'
    urlTemplate: '/{userId}'
  }
}

resource productsAPI 'Microsoft.ApiManagement/service/apis@2021-08-01' = {
  parent: apim
  name: 'products-api'
  properties: {
    subscriptionRequired: true
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    apiRevision: '1'
    isCurrent: true
    displayName: 'Products API'
    path: 'products'
    protocols: [
      'http'
      'https'
    ]
  }
}

resource getproducts 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  parent: productsAPI
  name:'Products-API'
  properties: {
    description: 'Use this operation to lookup users.'
    displayName: 'Lookup users'
    method: 'GET'
    urlTemplate: '/'
  }
}

resource getratingsAPI 'Microsoft.ApiManagement/service/apis@2021-08-01' = {
  parent: apim
  name: 'ratings-api'
  properties: {
    subscriptionRequired: true
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    apiRevision: '1'
    isCurrent: true
    displayName: 'Ratings GET API'
    path: 'ratings'
    protocols: [
      'http'
      'https'
    ]
  }
}

resource postratingsAPI 'Microsoft.ApiManagement/service/apis@2021-08-01' = {
  parent: apim
  name: 'rating-api'
  properties: {
    subscriptionRequired: true
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    apiRevision: '1'
    isCurrent: true
    displayName: 'Ratings POST API'
    path: 'rating'
    protocols: [
      'http'
      'https'
    ]
  }
}



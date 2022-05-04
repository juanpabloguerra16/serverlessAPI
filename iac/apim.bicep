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
    description: 'Use this operation to lookup products.'
    displayName: 'Lookup users'
    method: 'GET'
    urlTemplate: '/'
  }
}

resource getproduct 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  parent: productsAPI
  name:'Product-API'
  properties: {
    templateParameters: [
      {
        name: 'productId'
        description: 'Use this operation to lookup a specific product.'
        type: 'string'
        required: true
        values: []
      }
    ]
    description: 'Use this operation to lookup users.'
    displayName: 'Lookup users'
    method: 'GET'
    urlTemplate: '/{productId}'
  }
}

resource RatingsAPI 'Microsoft.ApiManagement/service/apis@2021-08-01' = {
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
    displayName: 'Ratings API'
    path: 'ratings'
    protocols: [
      'http'
      'https'
    ]
  }
}

resource createRating 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  parent: RatingsAPI
  name:'Create-Rating'
  properties: {
    description: 'Use this operation to create ratings.'
    displayName: 'Add ratings'
    method: 'POST'
    urlTemplate: 'CreateRatings/'
  }
}

resource getrating 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  parent: RatingsAPI
  name:'Rating-API'
  properties: {
    templateParameters: [
      {
        name: 'ratingId'
        description: 'Use this operation to lookup a specific rating.'
        type: 'string'
        required: true
        values: []
      }
    ]
    description: 'Use this operation to lookup ratings.'
    displayName: 'Lookup rating'
    method: 'GET'
    urlTemplate: '/{ratingId}'
  }
}

resource getratingbyuser 'Microsoft.ApiManagement/service/apis/operations@2021-08-01' = {
  parent: RatingsAPI
  name:'Rating-UserId-API'
  properties: {
    templateParameters: [
      {
        name: 'userId'
        description: 'Use this operation to lookup all ratings by userId.'
        type: 'string'
        required: true
        values: []
      }
    ]
    description: 'Use this operation to lookup ratings by userId.'
    displayName: 'Lookup rating'
    method: 'GET'
    urlTemplate: '/userid/{userId}'
  }
}

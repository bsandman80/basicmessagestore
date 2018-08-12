# Basic Message Store REST API
<!-- TOC depthFrom:1 depthTo:2 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Basic Message Store REST API](#basic-message-store-rest-api)
	- [Build instructions](#build-instructions)
	- [Authorization](#authorization)
	- [API overview](#api-overview)
	

<!-- /TOC -->

## Build instructions

```
git clone https://github.com/bsandman80/basicmessagestore.git
cd basicmessagestore/BasicMessageStore
dotnet build
dotnet run
```


## Authorization

### Instructions

The only actions allowed as anonymous is to register a user and create a token. 

#### Create user

```
curl -H "Content-Type: application/json" --request POST -d '{"username":"user1", "password":"password1"}' http://localhost:5000/api/account
```
#### Request a token

```
curl -H "content-type: application/json" --request POST -d '{"username":"user1","password":"password1"}' http://localhost:5000/api/token
```

#### Append the token as header on further requests

```
curl -H "content-type: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidXNlcjEiLCJuYmYiOjE1MzQxMTMxNDUsImV4cCI6MTUzNDExNDM0NSwiaXNzIjoiYmFzaWNNZXNzYWdlU3RvcmUiLCJhdWQiOiJiYXNpY01lc3NhZ2VTdG9yZUFQSSJ9.uZi6f4_cO-aygDNIcquL9lQhngb66nBHTHiPS_cMGVQ" --request GET http://localhost:5000/api/account

```

## API overview

### Tokens

#### Create an access token (POST)

A valid username and password must be provided in order to create an access token

``` http://localhost:5000/api/token ```
```javascript
  {
      "username": "Username1",
      "password": "Hemligt1"
  }
```


### Users

#### List all users (GET)

``` http://localhost:5000/api/account ```

#### Get user (GET)

``` http://localhost:5000/api/account/{id} ```

#### Register user (POST)

``` http://localhost:5000/api/account ```
```javascript
  {
      "username": "Username1",
      "password": "Hemligt1"
  }
```

#### Delete user (DELETE)

``` http://localhost:5000/api/account/{id} ```

### Messages

#### List all messages (GET)

``` http://localhost:5000/api/message ```

#### Get message (GET)

``` http://localhost:5000/api/message/{id} ```

#### Create new message (POST)

``` http://localhost:5000/api/message ```
```javascript
  {
      "header": "Header1",
      "body": "Body1"
  }
```

#### Update message (PUT)

``` http://localhost:5000/api/message/{id} ```
```javascript
  {
      "header": "New header",
      "body": "New body"
  }
```
#### Delete message (DELETE)

``` http://localhost:5000/api/message/{id} ```



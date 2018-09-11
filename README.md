# MongoDistributedCache

An implementation of IDistributedCache for MongoDB

## Installation

### .NET CLI

```bash
> dotnet add package MongoDistributedCache
```

### Package Manager

```bash
PM> Install-Package MongoDistributedCache
```

## Usage

```CSharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMongoDistributedCache(new MongoDistributedCacheOptions {
        Database = "DatabaseName",
        Collection = "CollectionName",
        Hosts = new List<string> {
            "Host:Port"
        },
        UserName = "UserName", //Optional
        Password = "Password", //Optional
        ExpiredRemovalInterval = TimeSpan.FromMinutes(5)
    });
}
```

## Options for MongoDistributedCacheOptions

| Property               | Type           | Description                                                | Required?    | Default Value |
|------------------------|----------------|------------------------------------------------------------|--------------|---------------|
| Database               | string         | The Database in Mongo to Target.                           | Yes          |               |
| Collection             | string         | The Collection in Mongo to Target.                         | Yes          |               |
| Hosts                  | List`<string`> | The hosts in the replica-set.                              | At least one |               |
| Username               | string         | The username required to connect to your replica-set.      | No           |               |
| Password               | string         | The password required to connect to your replica-set.      | No           |               |
| ExpiredRemovalInterval | TimeSpan?      | The interval to look for and remove expired cache entries. | No           | 3 Minutes     |
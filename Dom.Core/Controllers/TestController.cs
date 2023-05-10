using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Taxiverk.Core.Infrastructure.Api;
using Taxiverk.Domain.Model;
using Taxiverk.Domain.Mongo;
using Taxiverk.Infrastructure.MinioService;

namespace Taxiverk.Core.Controllers;

[AllowAnonymous]
[ApiController]
[Route("test")]
public class TestController : ApiController
{
    private readonly MinioBucketProvider<TestMinioBucket> _bucketProvider;
    private readonly MongoContext _mongoContext;

    public TestController(MinioBucketProvider<TestMinioBucket> bucketProvider, MongoContext mongoContext)
    {
        _bucketProvider = bucketProvider;
        _mongoContext = mongoContext;
    }

    [HttpPost("test")]
    public async Task<ApiData> Test()
    {
        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("Taxiverk")); 
        BsonSerializer.RegisterSerializer(objectSerializer);
        
        var spec = await _bucketProvider.PutByPath(@"C:\Users\pavlo\Downloads\spec-1.xlsx");
        
        var project = new BuildingProject
        {
            Id = Guid.NewGuid().ToString(),
            Client = new()
            {
                Fio = "Лихачёв Карл Русланович",
                Phone = "77024829563"
            },
            Engineer = new()
            {
                Fio = "Лихачёв Карл Русланович",
                Phone = "77024829563"
            },
            Manager = new()
            {
                Fio = "Лихачёв Карл Русланович",
                Phone = "77024829563"
            },
            Brigade = new()
            {
                Brigadier = new()
                {
                    Fio = "Лихачёв Карл Русланович",
                    Phone = "77024829563"
                },
                Members = new()
                {
                    new()
                    {
                        Fio = "Лихачёв Карл Русланович",
                        Phone = "77024829563"
                    },
                    new()
                    {
                        Fio = "Лихачёв Карл Русланович",
                        Phone = "77024829563"
                    },
                    new()
                    {
                        Fio = "Лихачёв Карл Русланович",
                        Phone = "77024829563"
                    },
                    new()
                    {
                        Fio = "Лихачёв Карл Русланович",
                        Phone = "77024829563"
                    },
                    new()
                    {
                        Fio = "Лихачёв Карл Русланович",
                        Phone = "77024829563"
                    }
                }
            },
            Materials = new ()
            {
                new()
                {
                    Comment = "bla bla bla",
                    Currencies = new ()
                    {
                        Date = DateTime.Now,
                        Dollar = 450,
                        Euro = 500,
                        Rub = 5
                    },
                    Specification = new()
                    {
                        File = spec,
                        Uploaded = DateTime.Now
                    }
                }
            }
        };

        await _mongoContext.Test.InsertOneAsync(project);

        return Success();
    }
}
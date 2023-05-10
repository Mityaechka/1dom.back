using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Taxiverk.Infrastructure.MinioService;

namespace Taxiverk.Domain.Model;

public class BuildingProject
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId EntityId { get; set; }
    
    public string Id { get; set; }
    
    public Person Manager { get; set; }
    public Person Client { get; set; }
    public Person Engineer { get; set; }

    public Brigade Brigade { get; set; }

    public KeyDates KeyDates { get; set; }

    public List<Material> Materials { get; set; }
}


public class Person
{
    public string Fio { get; set; }
    public string Phone { get; set; }
}

public class Brigade
{
    public Person Brigadier { get; set; }
    public List<Person> Members { get; set; }
}

public class KeyDates
{
    public List<KeyDate> Values { get; set; }
}

public class KeyDate
{
    public DateTime Date { get; set; }
    public KeyDateType Type { get; set; }
}

public enum KeyDateType
{
    ContractSigning,
    ControlMeasurement,
    Shipment,
    WorkStart,
    WorkEnd
}

public class Material
{
    public Specification Specification { get; set; }
    public Currencies Currencies { get; set; }
    public string Comment { get; set; }
}

public class Specification
{
    public FilePersisted File { get; set; }
    public DateTime Uploaded { get; set; }
}

public class Currencies
{
    public decimal Rub { get; set; }
    public decimal Euro { get; set; }
    public decimal Dollar { get; set; }
    public DateTime Date { get; set; }
}

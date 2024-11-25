using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace ODataWebApiApplication15.Models;

[DataContract(Name = "customer")]
public class Customer
{
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "customerType")]
    [JsonConverter(typeof(StringEnumConverter))]
    public CustomerType CustomerType { get; set; }

    [DataMember(Name = "orders")]
    public List<Order> Orders { get; set; } = new List<Order>();
}

[Flags]
[DataContract(Name = "customerType")]
public enum CustomerType
{
    [EnumMember(Value = "regular")]
    Regular = 1,

    [EnumMember(Value = "premium")]
    Premium = 2,

    [EnumMember(Value = "vip")]
    VIP = 4
}

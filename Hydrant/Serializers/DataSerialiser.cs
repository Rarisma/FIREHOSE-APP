using System.Text.Json.Serialization;
using HYDRANT.Definitions;
namespace HYDRANT.Serializers;
/// <summary>
/// This is required for AOT
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Data>))]
public partial class DataSerialiser : JsonSerializerContext;

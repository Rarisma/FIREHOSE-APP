using System.Text.Json.Serialization;
using HYDRANT.Definitions;
namespace HYDRANT.Serializers;
/// <summary>
/// This is required for AOT
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Article>))]
public partial class ArticleSerialiser : JsonSerializerContext;

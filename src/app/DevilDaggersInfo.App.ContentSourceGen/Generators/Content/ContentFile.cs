using DevilDaggersInfo.App.ContentSourceGen.Utils;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Content;

public class ContentFile
{
	public ContentFile(string fileNameWithExtension)
	{
		string fileExtension = Path.GetExtension(fileNameWithExtension);
		ContentType = fileExtension.GetContentTypeFromFileExtension();
		FileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);

		string contentTypeName = ContentType.GetContentTypeName();
		string fieldName = SourceBuilderUtils.ToField(FileName);
		string escapedLocalName = SourceBuilderUtils.ToEscapedLocal(FileName);
		string propertyName = SourceBuilderUtils.ToProperty(FileName);

		FieldInitializer = $"{fieldName} = content.TryGetValue(\"{FileName}\", out {contentTypeName}? {escapedLocalName}) ? {escapedLocalName} : null;";
		Field = $"private static {contentTypeName}? {fieldName};";
		Property = $"public static {contentTypeName} {propertyName} => {fieldName} ?? throw new InvalidOperationException(\"Content does not exist or has not been initialized.\");";
	}

	public string FileName { get; }
	public ContentType ContentType { get; }

	public string FieldInitializer { get; }
	public string Field { get; }
	public string Property { get; }
}

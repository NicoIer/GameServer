using Game001.CodeGenerator;

CodeGenerationContext context = CodeGenerationContext.Create(args);
CSharpSourceCatalog coreSources = CSharpSourceCatalog.Load(context.CoreDirectory);
ICodeGenerationStep[] steps =
{
    new EcsRegistrationGenerationStep(),
    new RoomMessageRegistrationGenerationStep(),
    new RoomHandlerGenerationStep(),
};

foreach (ICodeGenerationStep step in steps)
{
    CodeGenerationResult result = step.Execute(context, coreSources);
    Console.WriteLine(
        $"generator={step.Name} created={result.Created} updated={result.Updated} skipped={result.Skipped}");
}

using Mediator;

namespace MadWorldEU.Meishi.CurriculaVitae;

/// <summary>Gets the latest curriculum vitae, or <c>null</c> when there is none.</summary>
public sealed class GetDefaultCurriculumVitaeQuery : IRequest<GetDefaultCurriculumVitaeResult?>;

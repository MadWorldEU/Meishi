using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.EntityFrameworkCore;

namespace MadWorldEU.Meishi.Repositories;

public sealed class CurriculaVitaeRepository(MeishiContext context) : ICurriculaVitaeRepository
{
    public Task<CurriculumVitae?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        // Ids are version 7 GUIDs, which are time-ordered, so the highest id is the latest curriculum vitae
        return context.CurriculaVitae
            .Include(cv => cv.Experience)
            .Include(cv => cv.Education)
            .Include(cv => cv.SkillGroups)
            .AsSplitQuery()
            .OrderByDescending(cv => cv.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddOrUpdateAsync(CurriculumVitae curriculumVitae, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(curriculumVitae);

        // A tracked curriculum vitae (loaded through this repository) has its changes detected on save
        if (context.Entry(curriculumVitae).State == EntityState.Detached)
        {
            var exists = await context.CurriculaVitae.AnyAsync(cv => cv.Id == curriculumVitae.Id, cancellationToken);

            if (exists)
            {
                context.CurriculaVitae.Update(curriculumVitae);
            }
            else
            {
                context.CurriculaVitae.Add(curriculumVitae);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

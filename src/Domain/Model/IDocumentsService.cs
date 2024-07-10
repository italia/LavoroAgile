using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IDocumentsService
    {
        Task<FileStreamResult> GeneratePdfAsync(Guid id, CancellationToken cacncellation);

        Task<FileStreamResult> GeneratePdfValutazioneAsync(Guid id, CancellationToken cacncellation);
    }
}

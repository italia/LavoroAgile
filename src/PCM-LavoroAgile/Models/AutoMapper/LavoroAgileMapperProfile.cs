using AutoMapper;
using Domain.Model;
using Domain.Model.ExternalCommunications;
using PCM_LavoroAgile.Models.Search;
using System.Linq;

namespace PCM_LavoroAgile.Models.AutoMapper
{
    /// <summary>
    /// Definisce il mapping fra oggetti di dominio e ViewModel.
    /// </summary>
    public class LavoroAgileMapperProfile : Profile
    {

        /// <summary>
        /// Inizializza un nuovo <see cref="LavoroAgileMapperProfile"/>.
        /// </summary>
        public LavoroAgileMapperProfile()
        {
            CreateMap<Struttura, StrutturaViewModel>().ReverseMap().ForMember(s => s.StrutturaCompleta, opt => opt.Ignore());

            CreateMap<Accordo, AccordoHeaderViewModel>()
                .ForMember(a => a.DescriptiveCode, options => options.MapFrom<CodiceAccordoFormatterResolver>())
                .ForMember(a => a.ResponsabileAccordo, options => options.MapFrom(a => a.ResponsabileAccordo.NomeCognome))
                .ForMember(a => a.IdResponsabileAccordo, options => options.MapFrom(a => a.ResponsabileAccordo.Id))
                .ForMember(a => a.CapoIntermedio, options => options.MapFrom(a => a.CapoIntermedio.NomeCognome))
                .ForMember(a => a.CapoStruttura, options => options.MapFrom(a => a.CapoStruttura.NomeCognome))
                .ForMember(a => a.NomeCognome, options => options.MapFrom(a => a.Dipendente.NomeCognome))
                .ForMember(a => a.EmailResponsabileAccordo, options => options.MapFrom(a => a.ResponsabileAccordo.Email));

            CreateMap<TransmissionStatus, TransmissionStatusViewModel>();

            CreateMap<StoricoStato, StoricoStatoViewModel>();

            CreateMap<SegreteriaTecnica, SegreteriaTecnicaViewModel>().ReverseMap();

            CreateMap<SearchViewModel, AccordoSearch>();

            CreateMap<Accordo, AccordoViewModel>()
                .ForMember(
                    va => va.FasceDiContattabilita,
                    opt => opt.MapFrom(a => a.FasceDiContattabilita.Split(",", System.StringSplitOptions.TrimEntries).ToList()))
                .ForMember(
                    va => va.PianificazioneGiorniAccordo,
                    opt => opt.MapFrom(a => a.PianificazioneGiorniAccordo.Split(",", System.StringSplitOptions.TrimEntries).ToList()))
                .ForMember(a => a.DescriptiveCode, options => options.MapFrom<CodiceAccordoFormatterResolver>())
                .ReverseMap()
                .ForMember(
                    a => a.FasceDiContattabilita,
                    opt => opt.MapFrom(va => string.Join(",", va.FasceDiContattabilita)))
                .ForMember(
                    a => a.PianificazioneGiorniAccordo,
                    opt => opt.MapFrom(va => string.Join(",", va.PianificazioneGiorniAccordo)));
            CreateMap<Dirigente, DirigenteViewModel>().ReverseMap();
            CreateMap<Dipendente, DipendenteViewModel>().ReverseMap();
            CreateMap<Referente, ReferenteViewModel>().ReverseMap();

            CreateMap<AttivitaAccordo, AttivitaAccordoViewModel>().ReverseMap();            
        }
    }
}

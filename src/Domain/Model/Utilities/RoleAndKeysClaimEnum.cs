using System.ComponentModel;

namespace Domain.Model.Utilities
{
    // TODO: passare da enumeration a oggetto Enum
    public enum RoleAndKeysClaimEnum
    {
        [Description("Administrator")]
        ADMINISTRATOR,
        [Description("Utente")]
        UTENTE,
        [Description("Capo Struttura")]
        CAPO_STRUTTURA,
        [Description("Capo Intermedio")]
        CAPO_INTERMEDIO,
        [Description("Dirigente Responsabile")]
        DIRIGENTE_RESPONSABILE,
        [Description("Responsabile Accordo")]
        RESPONSABILE_ACCORDO,
        [Description("Referente Interno")]
        REFERENTE_INTERNO,
        [Description("Segreteria Tecnica")]
        SEGRETERIA_TECNICA,

        [Description("PCM_InternalOperativeRole_Utente")]
        KEY_CLAIM_UTENTE,
        [Description("PCM_InternalOperativeRole_CapoStruttura")]
        KEY_CLAIM_CAPO_STRUTTURA,
        [Description("PCM_InternalOperativeRole_CapoIntermedio")]
        KEY_CLAIM_CAPO_INTERMEDIO,
        [Description("PCM_InternalOperativeRole_DirigenteResponsabile")]
        KEY_CLAIM_DIRIGENTE_RESPONSABILE,
        [Description("PCM_InternalOperativeRole_ResponsabileAccordo")]
        KEY_CLAIM_RESPONSABILE_ACCORDO,
        [Description("PCM_InternalOperativeRole_ReferenteInterno")]
        KEY_CLAIM_REFERENTE_INTERNO,
        [Description("PCM_InternalOperativeRole_SegreteriaTecnica")]
        KEY_CLAIM_SEGRETERIA_TECNICA,


        [Description("PCM_InternalOperativeRole")]
        KEY_CLAIM_GENERIC,
        
        /// <summary>
        /// Utente interdetto (non può creare accordi)
        /// </summary>
        [Description("Interdetto")]
        INTERDICTED_USER
    }

}

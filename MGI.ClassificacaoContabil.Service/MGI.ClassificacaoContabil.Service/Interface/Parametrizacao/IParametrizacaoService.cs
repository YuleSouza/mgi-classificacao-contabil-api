﻿using DTO.Payload;
using Service.DTO.Parametrizacao;

namespace Service.Interface.Parametrizacao
{
    public interface IParametrizacaoService
    {
        #region [Cenario]
        Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<PayloadDTO> ConsultarParametrizacaoCenario();
        #endregion

        #region  [ESG Geral]
        Task<PayloadDTO> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao);
        Task<PayloadDTO> ConsultarParametrizacaoClassificacaoGeral();
        #endregion

        #region [ESG Excecao]
        Task<PayloadDTO> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao);
        Task<PayloadDTO> ConsultarParametrizacaoClassificacaoExcecao();
        #endregion
    }
}

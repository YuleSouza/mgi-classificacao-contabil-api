﻿using Dapper;

using Infra.Data;
using Infra.Interface;

using Service.DTO.Classificacao;
using Service.DTO.Filtros;
using Service.Repository.Classificacao;

namespace Repository.Classificacao
{
    public class ClassificacaoRepository : IClassificacaoRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSession _session;

        public ClassificacaoRepository(IUnitOfWork unitOfWork, DbSession session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
        }

        #region Contabil
        public async Task<bool> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classificacao_contabil (id_empresa, status, mesano_inicio, mesano_fim, uscriacao, dtcriacao) 
                                                                  values (:idempresa, :status, :dataInicial, :dataFinal, :uscriacao, sysdate)",
            new
            {
                idempresa = classificacao.IdEmpresa,
                status = classificacao.Status,
                dataInicial = classificacao.MesAnoInicio,
                dataFinal = classificacao.MesAnoFim,
                uscriacao = classificacao.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classificacao_contabil 
                                                                     set id_empresa      = :idempresa,
                                                                         status          = nvl(:status,status), 
                                                                         mesano_inicio   = :dataInicial,
                                                                         mesano_fim      = :dataFinal,
                                                                         usalteracao     = :usalteracao, 
                                                                         dtalteracao     = :dtalteracao
                                                                   where id_classificacao_contabil = :idclassificacao",
            new
            {
                idclassificacao = classificacao.IdClassificacaoContabil,
                idempresa = classificacao.IdEmpresa,
                status = classificacao.Status,
                dataInicial = classificacao.MesAnoInicio,
                dataFinal = classificacao.MesAnoFim,
                usalteracao = classificacao.Usuario?.UsuarioModificacao,
                dtalteracao = classificacao.Usuario?.DataModificacao
            });
            return result == 1;
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil()
        {

            var resultado = await _session.Connection.QueryAsync<ClassificacaoContabilDTO>($@"
                                                    select cc.id_classificacao_contabil  as IdClassificacaoContabil, 
                                                           cc.id_empresa                 as IdEmpresa,         
                                                           ltrim(rtrim(a.empnomfan))     as Nome,
                                                           cc.status                     as Status, 
                                                           cc.mesano_inicio              as MesAnoInicio,
                                                           cc.mesano_fim                 as MesAnoFim,
                                                           cc.dtcriacao                  as DataCriacao,
                                                           cc.uscriacao                  as UsuarioCriacao,
                                                           cc.dtalteracao                as DataModificacao,
                                                           cc.usalteracao                as UsuarioModificacao
                                                     from classificacao_contabil cc 
                                                     inner join corpora.empres a on cc.id_empresa = a.empcod
                                                     where 1 = 1
                                                     order by mesano_fim");
            return resultado;
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro)
        {
            string parametros = string.Empty;
            if (filtro.IdClassificacaoContabil > 0)
            {
                parametros += " and id_classificacao_contabil = :idclassificacao";
            }
            if (filtro.IdEmpresa > 0)
            {
                parametros += " and id_empresa = :idempresa";
            }
            if (filtro.IdProjeto > 0)
            {
                parametros += " and id_projeto = :idprojeto";
            }
            if (!string.IsNullOrEmpty(filtro.DataInicial))
            {
                parametros += " and mesano_inicio >= to_date(:dataInicial,'DD/MM/RRRR')";
            }
            if (!string.IsNullOrEmpty(filtro.DataFinal))
            {
                parametros += " and mesano_fim <= to_date(:dataFinal,'DD/MM/RRRR')";
            }
            var resultado = await _session.Connection.QueryAsync<ClassificacaoContabilDTO>($@"
                                                    select cc.id_classificacao_contabil  as IdClassificacaoContabil, 
                                                           cc.id_empresa                 as IdEmpresa,       
                                                           ltrim(rtrim(a.empnomfan))     as Nome,
                                                           cc.status                     as Status, 
                                                           cc.mesano_inicio              as MesAnoInicio,
                                                           cc.mesano_fim                 as MesAnoFim,
                                                           cc.dtcriacao                  as DataCriacao,
                                                           cc.uscriacao                  as UsuarioCriacao,
                                                           cc.dtalteracao                as DataModificacao,
                                                           cc.usalteracao                as UsuarioModificacao
                                                     from classificacao_contabil cc 
                                                     inner join corpora.empres a on cc.id_empresa = a.empcod
                                                     where 1 = 1
                                                     {parametros}
                                                     order by mesano_fim
                                        ", new
            {
                idclassificacao = filtro.IdClassificacaoContabil,
                idempresa = filtro.IdEmpresa,
                dataInicial = filtro.DataInicial,
                dataFinal = filtro.DataFinal
            });
            return resultado;
        }

        public async Task<bool> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classif_contabil_prj (id_classificacao_contabil, id_projeto, status, mesano_inicio, mesano_fim, uscriacao, dtcriacao) 
                                                                  values (:idclassificacaocontabil, :idprojeto, :status, :dataInicial, :dataFinal, :uscriacao, sysdate)",
            new
            {
                idclassificacaocontabil = projeto.IdClassificacaoContabil,
                idprojeto = projeto.IdProjeto,
                status = projeto.Status,
                dataInicial = projeto.MesAnoInicio,
                dataFinal = projeto.MesAnoFim,
                uscriacao = projeto.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classif_contabil_prj 
                                                                     set id_classificacao_contabil      = :idclassificacaocontabil,
                                                                         id_projeto                     = :idprojeto,
                                                                         status                         = nvl(:status,status), 
                                                                         mesano_inicio                  = :dataInicial,
                                                                         mesano_fim                     = :dataFinal,
                                                                         usalteracao                    = :usalteracao, 
                                                                         dtalteracao                    = :dtalteracao
                                                                   where id_classif_contabil_prj        = :idclassificacaoprojeto",
            new
            {
                idclassificacaoprojeto = projeto.IdClassificacaoContabilProjeto,
                idclassificacaocontabil = projeto.IdClassificacaoContabil,
                idprojeto = projeto.IdProjeto,
                status = projeto.Status,
                dataInicial = projeto.MesAnoInicio,
                dataFinal = projeto.MesAnoFim,
                usalteracao = projeto.Usuario?.UsuarioModificacao,
                dtalteracao = projeto.Usuario?.DataModificacao
            });
            return result == 1;
        }
        public async Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil()
        {

            var resultado = await _session.Connection.QueryAsync<ClassificacaoProjetoDTO>($@"
                                                    select cp.id_classif_contabil_prj                                    as IdClassificacaoContabil, 
                                                           cp.id_projeto                                                 as IdProjeto,  
                                                           to_char(p.prjcod, '00000') || ' - ' || ltrim(rtrim(p.prjnom)) as Nomeprojeto,
                                                           cp.status                                                     as Status, 
                                                           cp.mesano_inicio                                              as MesAnoInicio,
                                                           cp.mesano_fim                                                 as MesAnoFim,
                                                           cp.dtcriacao                                                  as DataCriacao,
                                                           cp.uscriacao                                                  as UsuarioCriacao,
                                                           cp.dtalteracao                                                as DataModificacao,
                                                           cp.usalteracao                                                as UsuarioModificacao
                                                     from classif_contabil_prj cp 
                                                     inner join servdesk.projeto p on p.prjcod = cp.id_projeto
                                                     where 1 = 1 
                                                     and p.prjsit = 'A'
                                                     order by mesano_fim");
            return resultado;
        }
        public async Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil(ClassificacaoContabilFiltro filtro)
        {
            string parametros = string.Empty;
            if (filtro.IdClassificacaoContabil > 0)
            {
                parametros += " and cp.id_classificacao_contabil = :idclassificacao";
            }
            if (filtro.IdProjeto > 0)
            {
                parametros += " and cp.id_projeto = :idprojeto";
            }
            if (!string.IsNullOrEmpty(filtro.DataInicial))
            {
                parametros += " and cp.mesano_inicio >= to_date(:dataInicial,'DD/MM/RRRR')";
            }
            if (!string.IsNullOrEmpty(filtro.DataFinal))
            {
                parametros += " and cp.mesano_fim <= to_date(:dataFinal,'DD/MM/RRRR')";
            }
            var resultado = await _session.Connection.QueryAsync<ClassificacaoProjetoDTO>($@"
                                                    select cp.id_classif_contabil_prj                                    as IdClassificacaoContabilProjeto, 
                                                           cp.id_classificacao_contabil                                  as IdClassificacaoContabil,
                                                           cp.id_projeto                                                 as IdProjeto,  
                                                           to_char(p.prjcod, '00000') || ' - ' || ltrim(rtrim(p.prjnom)) as Nomeprojeto,
                                                           cp.status                                                     as Status, 
                                                           cp.mesano_inicio                                              as MesAnoInicio,
                                                           cp.mesano_fim                                                 as MesAnoFim,
                                                           cp.dtcriacao                                                  as DataCriacao,
                                                           cp.uscriacao                                                  as UsuarioCriacao,
                                                           cp.dtalteracao                                                as DataModificacao,
                                                           cp.usalteracao                                                as UsuarioModificacao
                                                     from classif_contabil_prj cp 
                                                     inner join servdesk.projeto p on p.prjcod = cp.id_projeto
                                                     where 1 = 1 
                                                     and p.prjsit = 'A'
                                                     {parametros}
                                                     order by mesano_fim
                                        ", new
            {
                idclassificacao = filtro.IdClassificacaoContabil,
                idempresa = filtro.IdEmpresa,
                dataInicial = filtro.DataInicial,
                dataFinal = filtro.DataFinal
            });
            return resultado;
        }
        #endregion

        #region ESG
        public async Task<bool> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classificacao_esg (nome, status, uscriacao, dtcriacao) 
                                                                  values (:nome, :status, :uscriacao, sysdate)",
            new
            {
                nome = classificacao.Nome,
                status = classificacao.Status,
                uscriacao = classificacao.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classificacao_esg 
                                                                     set nome = nvl(:nome,nome),  
                                                                         status = nvl(:status,status), 
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = :dtalteracao
                                                                   where id_classificacao_esg = :idclassificacaoesg",
            new
            {
                idclassificacaoesg = classificacao.IdClassificacaoEsg,
                nome = classificacao.Nome,
                status = classificacao.Status,
                usalteracao = classificacao.Usuario?.UsuarioModificacao,
                dtalteracao = classificacao.Usuario?.DataModificacao
            });
            return result == 1;
        }
        public async Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg()
        {

            var resultado = await _session.Connection.QueryAsync<ClassificacaoEsgDTO>($@"
                                           select 
                                                id_classificacao_esg  as IdClassificacaoEsg,
                                                nome                as Nome,
                                                status              as Status,
                                                dtcriacao           as DataCriacao,
                                                uscriacao           as UsuarioCriacao,
                                                dtalteracao         as DataModificacao,
                                                usalteracao         as UsuarioModificacao
                                            from classificacao_esg
                                            where 1 = 1");
            return resultado;
        }
        public async Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro)
        {
            var parametros = string.Empty;
            if (filtro.IdClassificacaoEsg > 0)
            {
                parametros += $" and id_classificacao_esg = :idclassificacaoesg";
            }
            if (!string.IsNullOrEmpty(filtro.Nome))
            {
                parametros += " and upper(nome) like upper(:nome)";
            }
            if (!string.IsNullOrEmpty(filtro.Status))
            {
                parametros += $" and status = :status";
            }

            var resultado = await _session.Connection.QueryAsync<ClassificacaoEsgDTO>($@"
                                           select 
                                                id_classificacao_esg  as IdClassificacaoEsg,
                                                nome                as Nome,
                                                status              as Status,
                                                dtcriacao           as DataCriacao,
                                                uscriacao           as UsuarioCriacao,
                                                dtalteracao         as DataModificacao,
                                                usalteracao         as UsuarioModificacao
                                            from classificacao_esg
                                            where 1 = 1
                                           {parametros}
                                        ", new
            {
                idclassificacaoesg = filtro.IdClassificacaoEsg,
                nome = $"%{filtro.Nome}%",
                status = filtro.Status
            });
            return resultado;
        }
        #endregion
    }
}

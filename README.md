# ProjetoSaude
Projeto Saúde ~ Projeto Integrado B PUCc

## Entrega Contínua (Travis-Ci) 

[![Build Status](https://travis-ci.com/pedr0ni/ProjetoSaude.svg?token=EKgPpXgyA2FH2pmLC6vv&branch=master)](https://travis-ci.com/pedr0ni/ProjetoSaude)

## Branches

| Branch        | Descrição                |
| ------------- |:------------------------:|
| **master**    | Produção (Apresentação)  |
| dev           | Desenvolvimento da Equipe|

## Documentação

Toda documentação está feita inline no código fonte.

## Cronograma

[Veja o arquivo Cronograma.md](https://github.com/pedr0ni/ProjetoSaude/blob/master/Cronograma.md)

## Anotações

* Consegui resolver o problema do InMemory Database. Foi por conta do **IDatabaseContext** que estava sendo instanciado no **AppManager**
isso fazia com que sempre que chamado criava uma nova conexão e por consequencia um novo banco InMemory. Por isso não tinha problema quando usado
com o MySql. Isso também deu uma melhorada na performance porque não fica com várias conexões abertas no Banco. ~ Pedroni

## Autores

* Matheus Pedroni
* Daniel Oliveira

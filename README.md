# ProjetoSaude
Projeto Saúde ~ Projeto Integrado B PUCC

## Branches

| Branch        | Descrição                |
| ------------- |:------------------------:|
| **master**    | Produção (Apresentação)  |
| dev           | Desenvolvimento da Equipe|

## Documentação

Toda documentação está feita inline no código fonte.

## Cronograma

--

## Anotações

* Consegui resolver o problema do InMemory Database. Foi por conta do **IDatabaseContext** que estava sendo instanciado no **AppManager**
isso fazia com que sempre que chamado criava uma nova conexão e por consequencia um novo banco InMemory. Por isso não tinha problema quando usado
com o MySql. Isso também deu uma melhorada na performance porque não fica com várias conexões abertas no Banco. ~ Pedroni

## Autores

* Matheus Pedroni
* Daniel Oliveira
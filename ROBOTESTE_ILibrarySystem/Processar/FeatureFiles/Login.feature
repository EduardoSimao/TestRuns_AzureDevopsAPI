#language: pt-BR
Funcionalidade: Realizar Login
Para realizar o Login
Eu, como usuário,
Tenho que acessar a apgina inicial
E clicar no botão "Entrar"
E preencher campos "E-mail" e "Senha"
E clicar no botão "Entrar"

@tc:3
@RealizarLogin
Cenario: Realizar Login
	Dado que o usuario tenha acessado a pagina inicial
	E tenha clicado no botão Entrar
	Quando o mesmo inserir os dados necessarios corretamente
	E clicar no botão Entrar
	Então o login terá sido realizado com sucesso
(() => {
    const campo = document.getElementById("Descricao");
    const contador = document.getElementById("contadorDescricao");
    if (!campo || !contador) return;

    const atualizar = () => { contador.textContent = campo.value.length; };
    campo.addEventListener("input", atualizar);
    atualizar();
})();
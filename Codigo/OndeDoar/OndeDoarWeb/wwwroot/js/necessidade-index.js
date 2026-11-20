(() => {
    const busca = document.getElementById("filtroBusca");
    const status = document.getElementById("filtroStatus");
    const chips = document.querySelectorAll("[data-prioridade-filtro]");
    const cards = document.querySelectorAll(".necessidade-card");
    const semResultados = document.getElementById("semResultados");
    let prioridade = "todas";

    function aplicarFiltros() {
        const termo = busca.value.trim().toLowerCase();
        let visiveis = 0;

        cards.forEach(card => {
            const ok =
                card.dataset.busca.includes(termo) &&
                (prioridade === "todas" || card.dataset.prioridade === prioridade) &&
                (status.value === "todas" || card.dataset.status === status.value);
            card.hidden = !ok;
            if (ok) visiveis++;
        });

        // Só mostra a mensagem de "sem resultados" se existem cards e todos foram filtrados
        semResultados.hidden = cards.length === 0 ? false : visiveis > 0;
    }

    busca.addEventListener("input", aplicarFiltros);
    status.addEventListener("change", aplicarFiltros);
    chips.forEach(chip => chip.addEventListener("click", () => {
        prioridade = chip.dataset.prioridadeFiltro;
        chips.forEach(c => c.setAttribute("aria-pressed", String(c === chip)));
        aplicarFiltros();
    }));

    // Modal de exclusão (<dialog> nativo)
    const modal = document.getElementById("modalExcluir");

    document.querySelectorAll("[data-excluir]").forEach(botao => {
        botao.addEventListener("click", () => {
            document.getElementById("modalExcluirId").value = botao.dataset.id;
            modal.querySelector("form").action = botao.dataset.acao; // POST em /Necessidade/Delete/{id}
            document.getElementById("modalNome").textContent = botao.dataset.nome;
            document.getElementById("modalCategoria").textContent = botao.dataset.categoria;

            const rotulo = document.getElementById("modalStatus");
            rotulo.textContent = botao.dataset.status;
            rotulo.className = "badge-status status-" + botao.dataset.status.toLowerCase();

            const icone = botao.closest(".necessidade-card").querySelector(".card-icone");
            document.getElementById("modalIcone").innerHTML = icone.innerHTML;

            modal.showModal();
        });
    });

    modal.querySelectorAll("[data-fechar]").forEach(b => b.addEventListener("click", () => modal.close()));
    modal.addEventListener("click", e => { if (e.target === modal) modal.close(); });
})();
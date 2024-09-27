class Program
{
    // Valores dos compartimentos
    static double[] maxPesoCompartimentos = { 10000, 16000, 8000 }; // Capacidade de peso
    static double[] maxVolumeCompartimentos = { 6800, 8700, 5300 }; // Capacidade volumétrica
    static double totalPesoCompartimentos = maxPesoCompartimentos.Sum();

    // Valores das cargas
    static double[] pesos = { 18000, 15000, 23000, 12000 }; // Pesos por carga
    static double[] volumes = { 480, 650, 580, 390 }; // Volume por tonelada
    static double[] lucros = { 310, 380, 350, 285 }; // Lucro por tonelada

    static Random rand = new Random();

    // Parâmetros do AG
    const int populacaoSize = 500;
    const int geracoes = 2500;
    const double taxaMutacao = 0.01;

    static void Main()
    {
        List<double[]> populacao = IniciarPopulacao();

        for (int geracao = 0; geracao < geracoes; geracao++)
        {
            List<double[]> novaPopulacao = new List<double[]>();

            // Avaliação de cada indivíduo da população
            foreach (var individuo in populacao)
            {
                double[] descendencia = individuo;

                // Realiza a mutação com certa probabilidade
                if (rand.NextDouble() < taxaMutacao)
                {
                    descendencia = Mutacao(individuo);
                }

                novaPopulacao.Add(descendencia);
            }

            // Seleção dos melhores indivíduos
            novaPopulacao = novaPopulacao.OrderByDescending(ind => Fitness(ind)).Take(populacaoSize).ToList();
            populacao = novaPopulacao;

            if (geracao % 100 == 0)
            {
                Console.WriteLine($"Geração {geracao}: Melhor lucro = {Fitness(populacao[0])}");
            }
        }

        // Mostra o melhor resultado
        var melhorIndividuo = populacao[0];
        Console.WriteLine("Melhor solução encontrada:");
        for (int i = 0; i < melhorIndividuo.Length; i++)
        {
            Console.WriteLine($"Carga {i + 1}: {melhorIndividuo[i] * 100}%");
        }
        Console.WriteLine($"Lucro total: {Fitness(melhorIndividuo)}");
    }

    // Inicializa a população de soluções aleatórias
    static List<double[]> IniciarPopulacao()
    {
        List<double[]> populacao = new List<double[]>();
        for (int i = 0; i < populacaoSize; i++)
        {
            double[] individuo = new double[pesos.Length];
            for (int j = 0; j < pesos.Length; j++)
            {
                individuo[j] = rand.NextDouble(); // Porcentagem da carga que será aceita
            }
            populacao.Add(individuo);
        }
        return populacao;
    }

    // Função de avaliação que retorna o lucro total
    static double Fitness(double[] individuo)
    {
        double lucroTotal = 0;
        double[] pesoPorCompartimento = new double[maxPesoCompartimentos.Length];
        double[] volumePorCompartimento = new double[maxVolumeCompartimentos.Length];

        for (int i = 0; i < pesos.Length; i++)
        {
            double pesoCarga = individuo[i] * pesos[i];
            double volumeCarga = individuo[i] * pesos[i] * volumes[i];

            // Distribuir a carga proporcionalmente entre os compartimentos
            for (int j = 0; j < pesoPorCompartimento.Length; j++)
            {
                double proporcao = maxPesoCompartimentos[j] / totalPesoCompartimentos;
                double pesoAlocado = proporcao * pesoCarga;
                double volumeAlocado = proporcao * volumeCarga;

                // Verificar se o compartimento pode aceitar a carga
                if (pesoPorCompartimento[j] + pesoAlocado <= maxPesoCompartimentos[j] &&
                    volumePorCompartimento[j] + volumeAlocado <= maxVolumeCompartimentos[j])
                {
                    pesoPorCompartimento[j] += pesoAlocado;
                    volumePorCompartimento[j] += volumeAlocado;
                }
                else
                {
                    // Se não pode alocar a carga total, alocar o que é possível
                    double pesoDisponivel = maxPesoCompartimentos[j] - pesoPorCompartimento[j];
                    double volumeDisponivel = maxVolumeCompartimentos[j] - volumePorCompartimento[j];

                    // Calcular a fração que pode ser alocada com base na capacidade disponível
                    double pesoAlocavel = Math.Min(pesoDisponivel, pesoAlocado);
                    double volumeAlocavel = Math.Min(volumeDisponivel, volumeAlocado);

                    // Determinar a fração da carga que pode ser alocada
                    double cargaAlocavel = Math.Min(pesoAlocavel / pesoAlocado, volumeAlocavel / volumeAlocado);

                    // Alocar a fração possível
                    pesoPorCompartimento[j] += cargaAlocavel * pesoAlocado;
                    volumePorCompartimento[j] += cargaAlocavel * volumeAlocado;
                    lucroTotal += cargaAlocavel * individuo[i] * pesos[i] * lucros[i];
                }
            }

            // Adicionar lucro total da carga, considerando a fração alocada
            lucroTotal += individuo[i] * pesos[i] * lucros[i];
        }

        return lucroTotal;
    }

    // Função de mutação que altera aleatoriamente a porcentagem de uma carga
    static double[] Mutacao(double[] individuo)
    {
        int index = rand.Next(pesos.Length);
        double[] individuoMutado = (double[])individuo.Clone();
        individuoMutado[index] = rand.NextDouble();
        return individuoMutado;
    }
}

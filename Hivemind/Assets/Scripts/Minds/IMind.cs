public interface IMind
{
    void Execute();

    void Initiate(Ant ant);

    double Likelihood();

    IMind Clone();

    bool IsBusy();

    void GenerateUI();
}
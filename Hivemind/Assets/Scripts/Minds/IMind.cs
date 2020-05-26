public interface IMind
{
    void Execute();
    void Initiate(Ant ant);

    double Likelihood();

    IMind Clone();

    bool Equals(IMind mind);

    void Update(IMind mind);

    bool IsBusy();

    void GenerateUI();
}
using Assets.Scripts.Data;

public interface IMind
{
    void Execute();

    void Initiate(Ant ant);

    double Likelihood();

    IMind Clone();

    bool IsBusy();

    MindData GetData();

    void SetData(MindData mindData);
}
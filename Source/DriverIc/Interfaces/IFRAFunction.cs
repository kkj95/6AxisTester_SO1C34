using FZ4P.DriverIc.OISIC;

namespace FZ4P.DriverIc.Interfaces
{
    public interface IFRAFunction
    {
        int FRA_Addr { get; }

        void FRA_Echoboard_StartStop(int ch, StartStopType type);
        void AMA_Echoboard_StartStop(int ch, StartStopType type);
        bool Echo_Board_WhoAmI(int ch);
        void Echo_Board_SetParameter(Echo_ParamBase param);
        void Echo_Board_Select_Ch(int ch);
    }
}

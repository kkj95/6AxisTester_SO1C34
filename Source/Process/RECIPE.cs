using OpenCvSharp.Flann;
using OpenCvSharp.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace FZ4P
{
    public class Recipe
    {
        public CurrentPath Current { get; set; }
        public Condition Condition { get; set; }

        public Spec Spec { get; set; }
        public Model Model { get; set; }
        public Option Option { get; set; }
        public List<PassFail> PassFails { get; set; }
        public TotalYield yield { get; set; }
        public TestTime tt { get; set; }
        public VisionFile vsFile { get; set; }
        public RetryCount RetryCnt { get; set; }
        public Password pw { get; set; }
        public List<NewYield> YieldItem { get; set; }


        public Recipe()
        {

            Current = new CurrentPath();
            if (File.Exists(STATIC.CurrentPath))
                Current = DataIO.DeserializeXMLFileToObject<CurrentPath>(STATIC.CurrentPath);

            if (!Directory.Exists(STATIC.RootDir)) Directory.CreateDirectory(STATIC.RootDir);
            if (!Directory.Exists(STATIC.DataDir)) Directory.CreateDirectory(STATIC.DataDir);
            if (!Directory.Exists(STATIC.RecipeDir)) Directory.CreateDirectory(STATIC.RecipeDir);
            if (!Directory.Exists(STATIC.SpecDir)) Directory.CreateDirectory(STATIC.SpecDir);
            if (!Directory.Exists(STATIC.PackageDir)) Directory.CreateDirectory(STATIC.PackageDir);
            if (!Directory.Exists(STATIC.PackageDir + "AFPID\\")) Directory.CreateDirectory(STATIC.PackageDir + "AFPID\\");
            if (!Directory.Exists(STATIC.PackageDir + "OISXPID\\")) Directory.CreateDirectory(STATIC.PackageDir + "OISXPID\\");
            if (!Directory.Exists(STATIC.PackageDir + "OISYPID\\")) Directory.CreateDirectory(STATIC.PackageDir + "OISYPID\\");
            if (!Directory.Exists(STATIC.PackageDir + "OISFW\\")) Directory.CreateDirectory(STATIC.PackageDir + "OISFW\\");
            if (!Directory.Exists(STATIC.PackageDir + "OISBaseCal\\")) Directory.CreateDirectory(STATIC.PackageDir + "OISBaseCal\\");
            if (!Directory.Exists(STATIC.AFPIDDir)) Directory.CreateDirectory(STATIC.AFPIDDir);
            if (!Directory.Exists(STATIC.OISXPIDDir)) Directory.CreateDirectory(STATIC.OISXPIDDir);
            if (!Directory.Exists(STATIC.OISYPIDDir)) Directory.CreateDirectory(STATIC.OISYPIDDir);
            if (!Directory.Exists(STATIC.OISFWDir)) Directory.CreateDirectory(STATIC.OISFWDir);
            if (!Directory.Exists(STATIC.OISBaseCalDir)) Directory.CreateDirectory(STATIC.OISBaseCalDir);
            string res = string.Empty;
            res = STATIC.PKGRelease(STATIC.PackageDir, "*.rcp", STATIC.RecipeDir);
            if (res != string.Empty) Current.ConditionName = Path.GetFileName(res);
            res = STATIC.PKGRelease(STATIC.PackageDir, "*.spc", STATIC.SpecDir);
            if (res != string.Empty) Current.SpecName = Path.GetFileName(res);
            res = STATIC.PKGRelease(STATIC.PackageDir, "*.txt", STATIC.RootDir);

            res = STATIC.PKGRelease(STATIC.PackageDir + "AFPID\\", "*.txt", STATIC.AFPIDDir);
            if (res != string.Empty) Current.AFPidPath = res;
            res = STATIC.PKGRelease(STATIC.PackageDir + "OISXPID\\", "*.txt", STATIC.OISXPIDDir);
            if (res != string.Empty) Current.OISXPidPath = res;
            res = STATIC.PKGRelease(STATIC.PackageDir + "OISYPID\\", "*.txt", STATIC.OISYPIDDir);
            if (res != string.Empty) Current.OISYPidPath = res;
            res = STATIC.PKGRelease(STATIC.PackageDir + "OISFW\\", "*.ntbrst", STATIC.OISFWDir);
            if (res != string.Empty) Current.OISFWPath = res;
            res = STATIC.PKGRelease(STATIC.PackageDir + "OISBaseCal\\", "*.ntbrst", STATIC.OISBaseCalDir);
            if (res != string.Empty) Current.OISBaseCalPath = res;

            Current.SerializeToXMLFile(STATIC.CurrentPath);

            Condition = new Condition();
            Condition.InitAFSettling();
            if (File.Exists(STATIC.RecipeDir + Current.ConditionName))
            {
                Condition = DataIO.DeserializeXMLFileToObject<Condition>(STATIC.RecipeDir + Current.ConditionName);
                for (int i = 0; i < 50; i++)
                {
                    if (Condition.AFSettling[i] == null)
                    {
                        Condition.InitAFSettling();
                        break;
                    }
                }
            }
                

            Spec = new Spec();
            Spec.InitSpecList();
            if (File.Exists(STATIC.SpecDir + Current.SpecName))
            {
                Spec compare = new Spec();
                compare = DataIO.DeserializeXMLFileToObject<Spec>(STATIC.SpecDir + Current.SpecName);
                for (int i = 0; i < compare.specList.Count; i++)
                {
                    int index = Spec.specList.FindIndex(x => x.DisplayName == compare.specList[i].DisplayName);
                    if (index != -1)
                    {
                        Spec.specList[index].MinSpec = compare.specList[i].MinSpec;
                        Spec.specList[index].MaxSpec = compare.specList[i].MaxSpec;
                        Spec.specList[index].OnOff = compare.specList[i].OnOff;
                        Spec.specList[index].FailCnt = compare.specList[i].FailCnt;
                        Spec.specList[index].InspectionType = compare.specList[i].InspectionType;
                    }
                }
            }
            Model = new Model();

            Option = new Option();
            if (File.Exists(STATIC.OptionPath))
                Option = DataIO.DeserializeXMLFileToObject<Option>(STATIC.OptionPath);

            vsFile = new VisionFile();
            if (File.Exists(STATIC.VisionFileDir))
                vsFile = DataIO.DeserializeXMLFileToObject<VisionFile>(STATIC.VisionFileDir);
            else DataIO.SerializeToXMLFile(vsFile, STATIC.VisionFileDir);

            yield = new TotalYield();
            if (File.Exists(STATIC.YieldPath))
                yield = DataIO.DeserializeXMLFileToObject<TotalYield>(STATIC.YieldPath);

            YieldItem = new List<NewYield>();
            if (File.Exists(STATIC.YieldItemPath))
                YieldItem = DataIO.DeserializeXMLFileToObject<List<NewYield>>(STATIC.YieldItemPath);


            PassFails = new List<PassFail>();
            for (int i = 0; i < 2; i++)
            {
                PassFails.Add(new PassFail());
                for (int j = 0; j < (int)SpecItem.Length; j++) PassFails[i].Results.Add(new ResultItems());
            }
            tt = new TestTime();
            if (File.Exists(STATIC.TestTimeDir)) tt = DataIO.DeserializeXMLFileToObject<TestTime>(STATIC.TestTimeDir);

            RetryCnt = new RetryCount();

            pw = new Password();
            if (File.Exists(STATIC.PasswordDir))
                pw = DataIO.DeserializeXMLFileToObject<Password>(STATIC.PasswordDir);
        }
    }
    public class BaseRecipe
    {
        public List<object[]> Param = new List<object[]>();
        public string CurrentName { get; set; }
        public string FilePath { get; set; }
        public string[] ReadArry { get; set; }
        public bool bChange = false;
        public string InitDir { get; set; }
        public string Ext { get; set; }
        public virtual void Init(string current, string subDir)
        {
            if (!Directory.Exists(STATIC.BaseDir)) Directory.CreateDirectory(STATIC.BaseDir);
            InitDir = STATIC.BaseDir + subDir;
            Ext = Path.GetExtension(current);
            if (!Directory.Exists(InitDir)) Directory.CreateDirectory(InitDir);
            FilePath = STATIC.BaseDir + subDir + current;

            CurrentName = current;
            if (!File.Exists(FilePath)) Save();

            Read();
        }
        public virtual void Save(string filePath = "")
        {
        }
        public virtual void Read(string filePath = "")
        {
            if (!Directory.Exists(STATIC.RootDir)) Directory.CreateDirectory(STATIC.RootDir);
        }
        public virtual void SetParam()
        {
        }
        public virtual void SetParam(string key, string comment, object val)
        {
            for(int i = 0; i < Param.Count; i++)
            {
                if (Param[i][0].ToString() == key && Param[i][1].ToString() == comment)
                {
                    Param[i][2] = val;
                }
                if (Param[i][0].ToString() == key && comment == "")
                {
                    Param[i][1] = val;
                }
            }
        }
    }

    public class TestTime
    {
        public double CurrentST { get; set; } = 0;

        public double St { get; set; } = 0;
        public int Count { get; set; } = 0;
    }
    public class Option
    {

        [Option("Save Raw Data")] public bool SaveRawData { get; set; }
        [Option("Screen Capture")] public bool ScreenCapture { get; set; }
  //      [Option("Fixed Center")] public bool FixedCenter { get; set; }
        [Option("Write Result to DriverIC")] public bool WriteResultToDriverIC { get; set; }
        [Option("Safety Sensor Enable")] public bool SafeSensor { get; set; }
        [Option("AF Dir Reverse")] public bool AFDirReverse { get; set; }
        [Option("X Dir Reverse")] public bool XDirReverse { get; set; }
        [Option("Y Dir Reverse")] public bool YDirReverse { get; set; }
        [Option("XY Pos Reverse")] public bool XYPosReverse { get; set; }
        [Option("Socket Sensor Use")] public bool SocketSensorUse { get; set; }
        [Option("Settling Graph Visible")] public bool settlingGraphVisible { get; set; }
        [Option("Continue testing On Fail")] public bool ContinueTestingOnFail { get; set; }
      //  [Option("Fail Retry")] public bool FailRetry { get; set; }
        [Option("DryRun Mode")] public bool DryRunMode { get; set; }
        [Option("Barcode Use Flag")] public bool BarcodeUse { get; set; }
    }
    public class Condition
    {
        [Condition("ToDoList", "", "", "", "")] public List<string> ToDoList { get; set; } = new List<string>();
        //[Condition("PID", "OIS PID Ver.", "OIS Init", "", "_")] public int OISPIDVer { get; set; } = 11;

        [Condition("OIS FW", "Checksum", "OIS FW Donwload", "", "hex")] public string OISFWChecksum { get; set; } = "0x00000000";
        [Condition("OIS FW", "Program ID", "OIS FW Donwload", "OIS AutoBoot", "hex")] public string OISFWProgID { get; set; } = "0x00000000";
    //    [Condition("AF Position", "OIS Driving AF Position", "", "", "code")] public int AFBestPos { get; set; } = 2048;

        [Condition("AF Mechanical Stroke", "OL Min Code", "AF HallCalibration", "AF Only HallCalibration", "code")] public int AFOLMin { get; set; } = 0;
        [Condition("AF Mechanical Stroke", "OL Max Code", "AF HallCalibration", "AF Only HallCalibration", "code")] public int AFOLMax { get; set; } = 4095;
        [Condition("AF Mechanical Stroke", "OL Delay", "AF HallCalibration", "AF Only HallCalibration", "ms")] public int AFOLDelay { get; set; } = 100;
        [Condition("AF Mechanical Stroke", "OL Loop Count", "AF HallCalibration", "AF Only HallCalibration", "dec")] public int AFOLLoopCount { get; set; } = 10;
        [Condition("AF Mechanical Stroke", "On/Off", "AF HallCalibration", "AF Only HallCalibration", "1:On/0:Off")] public int AFMechaOnOff { get; set; } = 1;


        [Condition("OIS HallCalibration", "OIS Cal AF Position", "OIS HallCalibration", "", "code")] public int OISCalAFPos { get; set; } = 2048;
        [Condition("OIS HallCalibration", "W 0x60A0", "OIS HallCalibration", "", "hex")] public string OISCal60A0 { get; set; } = "0x06";
        [Condition("OIS HallCalibration", "W 0x61D2", "OIS HallCalibration", "", "hex")] public string OISCal61D2 { get; set; } = "0x50";
        [Condition("OIS HallCalibration", "W 0x61D3", "OIS HallCalibration", "", "hex")] public string OISCal61D3 { get; set; } = "0xB0";
        [Condition("OIS HallCalibration", "W 0x61D4", "OIS HallCalibration", "", "hex")] public string OISCal61D4 { get; set; } = "0x50";
        [Condition("OIS HallCalibration", "W 0x61D5", "OIS HallCalibration", "", "hex")] public string OISCal61D5 { get; set; } = "0x90";
        [Condition("OIS HallCalibration", "X Hall Min Spec", "OIS HallCalibration", "", "code")] public int XCalMinSpec { get; set; } = -7000;
        [Condition("OIS HallCalibration", "X Hall Max Spec", "OIS HallCalibration", "", "code")] public int XCalMaxSpec { get; set; } = 7000;
        [Condition("OIS HallCalibration", "Y Hall Min Spec", "OIS HallCalibration", "", "code")] public int YCalMinSpec { get; set; } = -7000;
        [Condition("OIS HallCalibration", "Y Hall Max Spec", "OIS HallCalibration", "", "code")] public int YCalMaxSpec { get; set; } = 7000;


        [Condition("Common", "OIS Drv AF Pos", "OIS X Scan", "OIS Y Scan", "code")] public int OISDrvAFPos { get; set; } = 40;
        [Condition("Common", "Drv AF Step", "AF Scan", "", "code")] public int iDrvAFStep { get; set; } = 40;
        [Condition("Common", "Drv X Step", "OIS X Scan", "", "code")] public int iDrvXStep { get; set; } = 400;
        [Condition("Common", "Drv Y Step", "OIS Y Scan", "", "code")] public int iDrvYStep { get; set; } = 400;
        [Condition("Common", "Drv Step Interval AF", "AF Scan", "", "msec")] public int iDrvStepIntervalZ { get; set; } = 40;
        [Condition("Common", "Drv Step interval X", "OIS X Scan", "", "msec")] public int iDrvStepIntervalX { get; set; } = 40;
        [Condition("Common", "Drv step Interval Y", "OIS Y Scan", "", "msec")] public int iDrvStepIntervalY { get; set; } = 40;
     
        [Condition("AF", "Drv Code Min", "AF Scan", "", "code")] public int iAFDrvCodeMin { get; set; } = 8;
        [Condition("AF", "Drv Code Max", "AF Scan", "", "code")] public int iAFDrvCodeMax { get; set; } = 4088;
        [Condition("AF", "Cross Axis Offset X", "AF Scan", "", "code")] public int iAFCrossOffsetX { get; set; } = 2048;
        [Condition("AF", "Cross Axis Offset Y", "AF Scan", "", "code")] public int iAFCrossOffsetY { get; set; } = 2048;
        [Condition("AF", "Plot Range", "AF Scan", "", "code")] public int iAFPlotRange { get; set; } = 2048;
        [Condition("AF", "OIS Servo Status", "AF Scan", "", "0:Off/1:On")] public int AFMoveOISServoStatus { get; set; } = 0;
        [Condition("AF", "Current Offset", "AF Scan", "", "mA")] public double AFCurrentOffset { get; set; } = -20;
        //[Condition("AF", "Code Range", "AF Scan", "", "code")] public int iAFCodeRange { get; set; } = 2048;
        //[Condition("AF", "Stroke Range", "AF Scan", "", "um")] public int iAFStrokeRange { get; set; } = 500;
        //[Condition("AF Settling", "Standby Code", "AF Settling", "", "code")] public int iAFStandbyCode { get; set; } = 8;
        //[Condition("AF Settling", "Jump Step Code", "AF Settling", "", "code")] public int iAFJumpStepCode { get; set; } = 2048;
        //[Condition("AF Settling", "Settling Criteria", "AF Settling", "", "%")] public double iAFSettlingCriteria { get; set; } = 5;


        [Condition("X", "Drv Code Min", "OIS X Scan", "", "code")] public int iXDrvCodeMin { get; set; } = 8;
        [Condition("X", "Drv Code Max", "OIS X Scan", "", "code")] public int iXDrvCodeMax { get; set; } = 4088;
        //  [Condition("X", "Cross Axis Offset", "OIS X Scan", "", "code")] public int iXCrossOffset { get; set; } = 2048;
        ////  [Condition("X", "Cross Axis Offset AF", "OIS X Scan", "", "code")] public int iXCrossOffsetAf { get; set; } = 2048;
        [Condition("X", "Plot Range", "OIS X Scan", "", "code")] public int iXPlotRange { get; set; } =  2048;
      //  [Condition("X", "Code Range", "OIS X Scan", "", "code")] public int iXCodeRange { get; set; } = 2048;
        [Condition("X", "stroke Range", "OIS X Scan", "", "um")] public int iXStrokeRange { get; set; } = 500;

        [Condition("Y", "Drv Code Min", "OIS Y Scan", "", "code")] public int iYDrvCodeMin { get; set; } = 8;
        [Condition("Y", "Drv Code Max", "OIS Y Scan", "", "code")] public int iYDrvCodeMax { get; set; } = 4088;
        //    //[Condition("Y2", "Drv Code Min", "OIS Y Scan", "", "code")] public int iY2DrvCodeMin { get; set; } = 8;
        //    //[Condition("Y2", "Drv Code Max", "OIS Y Scan", "", "code")] public int iY2DrvCodeMax { get; set; } = 4088;

        //    [Condition("Y", "Cross Axis Offset", "OIS Y Scan", "", "code")] public int iYCrossOffset { get; set; } = 2048;
        ////    [Condition("Y", "Cross Axis Offset AF", "OIS Y Scan", "", "code")] public int iYCrossOffsetAf { get; set; } = 2048;
        [Condition("Y", "Plot Range", "OIS Y Scan", "", "code")] public int iYPlotRange { get; set; } = 2048;
    //    [Condition("Y", "Code Range", "OIS Y Scan", "", "code")] public int iYCodeRange { get; set; } = 2048;
        [Condition("Y", "Stroke Range", "OIS Y Scan", "", "um")] public int iYStrokeRange { get; set; } = 500;

        //[Condition("AF OL Aging", "Frequency", "AF OpenLoopAging", "", "Hz")] public int AFOpenLoopFreq { get; set; } = 10;
        //[Condition("AF OL Aging", "Count", "AF OpenLoopAging", "", "-")] public int AFOpenLoopCount { get; set; } = 10;


        [Condition("XYZ Aging", "Frequency", "XYZ Aging", "", "-")] public int CLAgingFreq { get; set; } = 10;
        [Condition("XYZ Aging", "Count", "XYZ Aging", "", "-")] public int CLAgingCount { get; set; } = 30;
    
        [Condition("XYZ Aging", "AF Start", "XYZ Aging", "", "-")] public int CLAgingAFMin { get; set; } = 200;
        [Condition("XYZ Aging", "AF End", "XYZ Aging", "", "-")] public int CLAgingAFMax { get; set; } = 3000;
        [Condition("XYZ Aging", "OIS Start", "XYZ Aging", "", "-")] public int CLAgingOISMin { get; set; } = 100;
        [Condition("XYZ Aging", "OIS End", "XYZ Aging", "", "-")] public int CLAgingOISMax { get; set; } = 4000;


        [Condition("AF Aging", "Count", "AF Aging", "", "-")] public int AFSCanAgingCount { get; set; } = 3;
        [Condition("AF Aging", "Step", "AF Aging", "", "-")] public int AFScanAgingStep { get; set; } = 256;
        [Condition("AF Aging", "delay", "AF Aging", "", "-")] public int AFScanAgingDelay { get; set; } = 15;
      

        [Condition("OIS Linearity Comp", "Steps", "OIS LinearityCompensation", "", "code")] public int OISLincompStep { get; set; } = 32;
        [Condition("OIS Linearity Comp", "Code Margin", "OIS LinearityCompensation", "", "code")] public int OISLincompCodeMargin { get; set; } = 100;
        [Condition("OIS EPA", "X EPA POS", "OIS LinearityCompensation", "", "um")] public int OISXEPAPos { get; set; } = 0;
        [Condition("OIS EPA", "X EPA NEG", "OIS LinearityCompensation", "", "um")] public int OISXEPANeg { get; set; } = 0;
        [Condition("OIS EPA", "Y EPA POS", "OIS LinearityCompensation", "", "um")] public int OISYEPAPos { get; set; } = 8;
        [Condition("OIS EPA", "Y EPA NEG", "OIS LinearityCompensation", "", "um")] public int OISYEPANeg { get; set; } = 0;

        [Condition("AF PM", "AF Step", "AF Phase Margin", "", "%")] public int iAFFRAstep { get; set; } = 5;
        [Condition("AF PM", "AF Chirp from", "AF Phase Margin", "", "Hz")] public int iAFChirpFrom { get; set; } = 250;
        [Condition("AF PM", "AF Chirp to", "AF Phase Margin", "", "Hz")] public int iAFChirpTo { get; set; } = 100;
        [Condition("AF PM", "AF Drv Amp", "AF Phase Margin", "", "mV")] public double iAFAmplitude { get; set; } = 75;
        [Condition("AF PM", "AF Gain Th", "AF Phase Margin", "", "_")] public int PMAFGainTH { get; set; } = 0;
        [Condition("AF PM", "AF Position", "AF Phase Margin", "", "code")] public int AfPosPM { get; set; } = 2048;

        [Condition("AF GM", "Chirp From", "AF Gain Margin", "", "Hz")] public int AFGMStartFreq { get; set; } = 2000;
        [Condition("AF GM", "Chirp To", "AF Gain Margin", "", "Hz")] public int AFGMEndFreq { get; set; } = 300;
        [Condition("AF GM", "Step", "AF Gain Margin", "", "%")] public int AFGMStep { get; set; } = 15;
        [Condition("AF GM", "Amp", "AF Gain Margin", "", "mV")] public int AFGMamp { get; set; } = 2048;
        [Condition("AF GM", "AF Position", "AF Gain Margin", "", "code")] public int AFPosGM { get; set; } = 40;

        [Condition("AF LoopGain", "Amp", "AF LoopGain", "", "mV")] public int AFLoopGainAmp { get; set; } = 40;
        [Condition("AF LoopGain", "Freq", "AF LoopGain", "", "Hz")] public int AFLoopGainFreq { get; set; } = 10;
        [Condition("AF LoopGain", "AF Position", "AF LoopGain", "", "code")] public int AFLoopGainPos { get; set; } = 2048;

        [Condition("OIS PM", "X Insp Count", "OIS Phase Margin", "", "dec")] public int XPMInspCnt { get; set; } = 50;
        [Condition("OIS PM", "X Chirp from", "OIS Phase Margin", "", "Hz")] public int iXChirpFrom { get; set; } = 250;
        [Condition("OIS PM", "X Chirp to", "OIS Phase Margin", "", "Hz")] public int iXChirpTo { get; set; } = 20;
        [Condition("OIS PM", "X Drv Amp", "OIS Phase Margin", "", "mV")] public int iXAmplitude { get; set; } = 60;

        [Condition("OIS PM", "Y Insp Count", "OIS Phase Margin", "", "dec")] public int YPMInspCnt { get; set; } = 50;
        [Condition("OIS PM", "Y Chirp from", "OIS Phase Margin", "", "Hz")] public int iYChirpFrom { get; set; } = 250;
        [Condition("OIS PM", "Y Chirp to", "OIS Phase Margin", "", "Hz")] public int iYChirpTo { get; set; } = 100;
        [Condition("OIS PM", "Y Drv Amp", "OIS Phase Margin", "", "mV")] public int iYAmplitude { get; set; } = 75;
        [Condition("OIS PM", "AF Position", "OIS Phase Margin", "", "code")] public int AFPosOISPM { get; set; } = 2048;

        [Condition("OIS GM", "X Insp Count", "OIS Gain Margin", "", "dec")] public int XGMInspCnt { get; set; } = 50;
        [Condition("OIS GM", "X Chirp from", "OIS Gain Margin", "", "Hz")] public int iXChirpFromGM { get; set; } = 250;
        [Condition("OIS GM", "X Chirp to", "OIS Gain Margin", "", "Hz")] public int iXChirpToGM { get; set; } = 20;
        [Condition("OIS GM", "X Drv Amp", "OIS Gain Margin", "", "mV")] public int iXAmplitudeGM { get; set; } = 60;

        [Condition("OIS GM", "Y Insp Count", "OIS Gain Margin", "", "dec")] public int YGMInspCnt { get; set; } = 50;
        [Condition("OIS GM", "Y Chirp from", "OIS Gain Margin", "", "Hz")] public int iYChirpFromGM { get; set; } = 250;
        [Condition("OIS GM", "Y Chirp to", "OIS Gain Margin", "", "Hz")] public int iYChirpToGM { get; set; } = 100;
        [Condition("OIS GM", "Y Drv Amp", "OIS Gain Margin", "", "mV")] public int iYAmplitudeGM { get; set; } = 75;
        [Condition("OIS GM", "AF Position", "OIS Gain Margin", "", "code")] public int AFPosOISGM { get; set; } = 2048;

       
        [Condition("OIS LoopGain", "X Amp", "OIS LoopGain", "", "mV")] public int OISXLoopGainAmp { get; set; } = 40;
        [Condition("OIS LoopGain", "X Freq", "OIS LoopGain", "", "Hz")] public int OISXLoopGainFreq { get; set; } = 10;
        [Condition("OIS LoopGain", "Y Amp", "OIS LoopGain", "", "mV")] public int OISYLoopGainAmp { get; set; } = 40;
        [Condition("OIS LoopGain", "Y Freq", "OIS LoopGain", "", "Hz")] public int OISYLoopGainFreq { get; set; } = 10;
        [Condition("OIS LoopGain", "AF Position", "OIS LoopGain", "", "code")] public int OISLoopGainAFPos { get; set; } = 2048;

        [Condition("AF Tilt", "Ref Code", "AF Scan", "", "code")] public int TiltRefCode { get; set; } = 1000;
        [Condition("AF Tilt", "Min Range", "AF Scan", "", "code")] public int TiltMinCode { get; set; } = 200;
        [Condition("AF Tilt", "Max Range", "AF Scan", "", "code")] public int TiltMaxCode { get; set; } = 3900;


        [Condition("AF Dynamic Tilt(Regional)", "Ref Code", "AF Scan", "", "code")] public int DynamincTiltRefCode { get; set; } = 689;
        [Condition("AF Dynamic Tilt(Regional)", "Range", "AF Scan", "", "um")] public double DynamicTiltRange { get; set; } = 50;
  


        [Condition("AF Linearity", "Min Range", "AF Scan", "", "code")] public int AFLinMinRange { get; set; } = 200;
        [Condition("AF Linearity", "Max Range", "AF Scan", "", "code")] public int AFLinMaxRange { get; set; } = 3900;
        [Condition("AF Linearity", "Min Step", "AF Scan", "", "_")] public int AFLinMinStep { get; set; } = 0;
        [Condition("AF Linearity", "Max Step", "AF Scan", "", "_")] public int AFLinMaxStep { get; set; } = 0;
        [Condition("AF Linearity", "Min Stroke", "AF Scan", "", "um")] public double AFLinMinStroke { get; set; } = -310;
        [Condition("AF Linearity", "Max Stroke", "AF Scan", "", "um")] public double AFLinMaxStroke { get; set; } = 310;
        [Condition("AF Linearity", "Mode", "AF Scan", "", "0:CodeRange / 1:Step / 2:um")] public int AFLinMode { get; set; } = 0;

        [Condition("AF Hysteresis", "Min Range", "AF Scan", "", "code")] public int AFHysMinRange { get; set; } = 200;
        [Condition("AF Hysteresis", "Max Range", "AF Scan", "", "code")] public int AFHysMaxRange { get; set; } = 3900;
        [Condition("AF Hysteresis", "Min Step", "AF Scan", "", "_")] public int AFHysMinStep { get; set; } = 0;
        [Condition("AF Hysteresis", "Max Step", "AF Scan", "", "_")] public int AFhysMaxStep { get; set; } = 0;
        [Condition("AF Hysteresis", "Min Stroke", "AF Scan", "", "um")] public double AFHysMinStroke { get; set; } = -310;
        [Condition("AF Hysteresis", "Max Stroke", "AF Scan", "", "um")] public double AFHysMaxStroke { get; set; } = 310;
        [Condition("AF Hysteresis", "Mode", "AF Scan", "", "0:CodeRange / 1:Step / 2:um")] public int AFHysMode { get; set; } = 0;

        [Condition("AF Current", "Min Range", "AF Scan", "", "code")] public int AFCurrMinRange { get; set; } = 200;
        [Condition("AF Current", "Max Range", "AF Scan", "", "code")] public int AFCurrMaxRange { get; set; } = 3900;
        [Condition("AF Current", "Min Step", "AF Scan", "", "_")] public int AFCurrMinStep { get; set; } = 0;
        [Condition("AF Current", "Max Step", "AF Scan", "", "_")] public int AFCurrMaxStep { get; set; } = 0;
        [Condition("AF Current", "Min Stroke", "AF Scan", "", "um")] public double AFCurrMinStroke { get; set; } = -310;
        [Condition("AF Current", "Max Stroke", "AF Scan", "", "um")] public double AFCurrMaxStroke { get; set; } = 310;
        [Condition("AF Current", "Mode", "AF Scan", "", "0:CodeRange / 1:Step / 2:um")] public int AFCurrMode { get; set; } = 0;

        [Condition("X Linearity", "Min Range", "OIS X Scan", "", "code")] public int XLinMinRange { get; set; } = 648;
        [Condition("X Linearity", "Max Range", "OIS X Scan", "", "code")] public int XLinMaxRange { get; set; } = 3448;
        [Condition("X Linearity", "Min Step", "OIS X Scan", "", "_")] public int XLinMinStep { get; set; } = 0;
        [Condition("X Linearity", "Max Step", "OIS X Scan", "", "_")] public int XLinMaxStep { get; set; } = 0;
        [Condition("X Linearity", "Min Stroke", "OIS X Scan", "", "um")] public double XLinMinStroke { get; set; } = -310;
        [Condition("X Linearity", "Max Stroke", "OIS X Scan", "", "um")] public double XLinMaxStroke { get; set; } = 310;
        [Condition("X Linearity", "Mode", "OIS X Scan", "", "0:CodeRange / 1:Step / 2:um")] public int XLinMode { get; set; } = 0;

        [Condition("X Hysteresis", "Min Range", "OIS X Scan", "", "code")] public int XHysMinRange { get; set; } = 648;
        [Condition("X Hysteresis", "Max Range", "OIS X Scan", "", "code")] public int XHysMaxRange { get; set; } = 3448;
        [Condition("X Hysteresis", "Min Step", "OIS X Scan", "", "_")] public int XHysMinStep { get; set; } = 0;
        [Condition("X Hysteresis", "Max Step", "OIS X Scan", "", "_")] public int XHysMaxStep { get; set; } = 0;
        [Condition("X Hysteresis", "Min Stroke", "OIS X Scan", "", "um")] public double XHysMinStroke { get; set; } = -310;
        [Condition("X Hysteresis", "Max Stroke", "OIS X Scan", "", "um")] public double XHysMaxStroke { get; set; } = 310;
        [Condition("X Hysteresis", "Mode", "OIS X Scan", "", "0:CodeRange / 1:Step / 2:um")] public int XHysMode { get; set; } = 0;

        [Condition("X Current", "Min Range", "OIS X Scan", "", "code")] public int XCurrMinRange { get; set; } = 200;
        [Condition("X Current", "Max Range", "OIS X Scan", "", "code")] public int XCurrMaxRange { get; set; } = 3900;
        [Condition("X Current", "Min Step", "OIS X Scan", "", "_")] public int XCurrMinStep { get; set; } = 0;
        [Condition("X Current", "Max Step", "OIS X Scan", "", "_")] public int XCurrMaxStep { get; set; } = 0;
        [Condition("X Current", "Min Stroke", "OIS X Scan", "", "um")] public double XCurrMinStroke { get; set; } = -310;
        [Condition("X Current", "Max Stroke", "OIS X Scan", "", "um")] public double XCurrMaxStroke { get; set; } = 310;
        [Condition("X Current", "Mode", "OIS X Scan", "", "0:CodeRange / 1:Step / 2:um")] public int XCurrMode { get; set; } = 0;

        [Condition("Xy XTalk", "Min Range", "OIS X Scan", "", "code")] public int X_XtalkMinRange { get; set; } = 200;
        [Condition("Xy XTalk", "Max Range", "OIS X Scan", "", "code")] public int X_XtalkMaxRange { get; set; } = 3900;
        [Condition("Xy XTalk", "Min Step", "OIS X Scan", "", "_")] public int X_XtalkMinStep { get; set; } = 0;
        [Condition("Xy XTalk", "Max Step", "OIS X Scan", "", "_")] public int X_XtalkMaxStep { get; set; } = 0;
        [Condition("Xy XTalk", "Min Stroke", "OIS X Scan", "", "um")] public double X_XtalkMinStroke { get; set; } = -310;
        [Condition("Xy XTalk", "Max Stroke", "OIS X Scan", "", "um")] public double X_XtalkMaxStroke { get; set; } = 310;
        [Condition("Xy XTalk", "Mode", "OIS X Scan", "", "0:CodeRange / 1:Step / 2:um")] public int X_XtalkMode { get; set; } = 0;


        [Condition("X Sensitivity", "Max Stroke", "OIS X Scan", "", "um")] public double XSensitivityMaxStroke { get; set; } = 300;
        [Condition("X Sensitivity", "Min Stroke", "OIS X Scan", "", "um")] public double XSensitivityMinStroke { get; set; } = -300;


        [Condition("Y Linearity", "Min Range", "OIS Y Scan", "", "code")] public int YLinMinRange { get; set; } = 648;
        [Condition("Y Linearity", "Max Range", "OIS Y Scan", "", "code")] public int YLinMaxRange { get; set; } = 3448;
        [Condition("Y Linearity", "Min Step", "OIS Y Scan", "", "_")] public int YLinMinStep { get; set; } = 0;
        [Condition("Y Linearity", "Max Step", "OIS Y Scan", "", "_")] public int YLinMaxStep { get; set; } = 0;
        [Condition("Y Linearity", "Min Stroke", "OIS Y Scan", "", "um")] public double YLinMinStroke { get; set; } = -310;
        [Condition("Y Linearity", "Max Stroke", "OIS Y Scan", "", "um")] public double YLinMaxStroke { get; set; } = 310;
        [Condition("Y Linearity", "Mode", "OIS Y Scan", "", "0:CodeRange / 1:Step / 2:um")] public int YLinMode { get; set; } = 0;

        [Condition("Y Hysteresis", "Min Range", "OIS Y Scan", "", "code")] public int YHysMinRange { get; set; } = 648;
        [Condition("Y Hysteresis", "Max Range", "OIS Y Scan", "", "code")] public int YHysMaxRange { get; set; } = 3448;
        [Condition("Y Hysteresis", "Min Step", "OIS Y Scan", "", "_")] public int YHysMinStep { get; set; } = 0;
        [Condition("Y Hysteresis", "Max Step", "OIS Y Scan", "", "_")] public int YHysMaxStep { get; set; } = 0;
        [Condition("Y Hysteresis", "Min Stroke", "OIS Y Scan", "", "_")] public double YHysMinStroke { get; set; } = -310;
        [Condition("Y Hysteresis", "Max Stroke", "OIS Y Scan", "", "_")] public double YHysMaxStroke { get; set; } = 310;
        [Condition("Y Hysteresis", "Mode", "OIS Y Scan", "", "0:CodeRange / 1:Step / 2:um")] public int YHysMode { get; set; } = 0;

        [Condition("Y Current", "Min Range", "OIS Y Scan", "", "code")] public int YCurrMinRange { get; set; } = 200;
        [Condition("Y Current", "Max Range", "OIS Y Scan", "", "code")] public int YCurrMaxRange { get; set; } = 3900;
        [Condition("Y Current", "Min Step", "OIS Y Scan", "", "_")] public int YCurrMinStep { get; set; } = 0;
        [Condition("Y Current", "Max Step", "OIS Y Scan", "", "_")] public int YCurrMaxStep { get; set; } = 0;
        [Condition("Y Current", "Min Stroke", "OIS Y Scan", "", "um")] public double YCurrMinStroke { get; set; } = -310;
        [Condition("Y Current", "Max Stroke", "OIS Y Scan", "", "um")] public double YCurrMaxStroke { get; set; } = 310;
        [Condition("Y Current", "Mode", "OIS Y Scan", "", "0:CodeRange / 1:Step / 2:um")] public int YCurrMode { get; set; } = 0;

        [Condition("Yx XTalk", "Min Range", "OIS Y Scan", "", "code")] public int Y_XtalkMinRange { get; set; } = 200;
        [Condition("Yx XTalk", "Max Range", "OIS Y Scan", "", "code")] public int Y_XtalkMaxRange { get; set; } = 3900;
        [Condition("Yx XTalk", "Min Step", "OIS Y Scan", "", "_")] public int Y_XtalkMinStep { get; set; } = 0;
        [Condition("Yx XTalk", "Max Step", "OIS Y Scan", "", "_")] public int Y_XtalkMaxStep { get; set; } = 0;
        [Condition("Yx XTalk", "Min Stroke", "OIS Y Scan", "", "um")] public double Y_XtalkMinStroke { get; set; } = -310;
        [Condition("Yx XTalk", "Max Stroke", "OIS Y Scan", "", "um")] public double Y_XtalkMaxStroke { get; set; } = 310;
        [Condition("Yx XTalk", "Mode", "OIS Y Scan", "", "0:CodeRange / 1:Step / 2:um")] public int Y_XtalkMode { get; set; } = 0;


        [Condition("Y Sensitivity", "Max Stroke", "OIS Y Scan", "", "um")] public double YSensitivityMaxStroke { get; set; } = 300;
        [Condition("Y Sensitivity", "Min Stroke", "OIS Y Scan", "", "um")] public double YSensitivityMinStroke { get; set; } = -300;

        [Condition("X/Y Servo Decenter", "AF Position", "X/Y Servo Decenter", "", "code")] public int ServoDecenterAFPos { get; set; } = 1252;
        [Condition("X/Y Servo Decenter", "Delay", "X/Y Servo Decenter", "", "ms")] public int ServoDecenterDelay { get; set; } = 100;



        [Condition("AF Rated Stroke", "Min Code", "AF Rated Stroke", "", "code")] public int AFRatedStrokeMinCode { get; set; } = 231;
        [Condition("AF Rated Stroke", "Max Code", "AF Rated Stroke", "", "code")] public int AFRatedStrokeMaxCode { get; set; } = 3841;
        [Condition("AF Rated Stroke", "Delay", "AF Rated Stroke", "", "ms")] public int AFRatedStrokeDelay { get; set; } = 200;

        [Condition("AF HAT", "Stroke", "AF HAT", "", "um")] public double AFHATStroke { get; set; } = 300;
        [Condition("AF HAT", "Move Delay", "AF HAT", "", "ms")] public int AFHATMoveDelay { get; set; } = 200;

        [Condition("OIS HAT", "Stroke", "OIS HAT", "", "um")] public double OISHATStroke { get; set; } = 300;
        [Condition("OIS HAT", "Move Delay", "OIS HAT", "", "ms")] public int OISHATMoveDelay { get; set; } = 200;

        [Condition("AF RunUp Test", "Start Code1", "AF RunUp Test", "", "code")] public int AFRunUpStartCode { get; set; } = 0;
        [Condition("AF RunUp Test", "End Code1", "AF RunUp Test", "", "code")] public int AFRunUpEndCode { get; set; } = 2100;
        [Condition("AF RunUp Test", "Start Code2", "AF RunUp Test", "", "code")] public int AFRunUpStartCode2 { get; set; } = 0;
        [Condition("AF RunUp Test", "End Code2", "AF RunUp Test", "", "code")] public int AFRunUpEndCode2 { get; set; } = 2100;
        [Condition("AF RunUp Test", "Gain Mode", "AF RunUp Test", "", "code")] public int AFRunUpGain { get; set; } = 0;

        [Condition("OIS RunUp Test", "Find X Diff Time", "OIS RunUp Test", "", "ms")] public int OISRunUpFindXDiffTimeX { get; set; } = 50;
        [Condition("OIS RunUp Test", "Find Y Diff Time", "OIS RunUp Test", "", "ms")] public int OISRunUpFindXDiffTimeY { get; set; } = 50;

        [Condition("OIS Decenter Calibration", "AF Position", "OIS Decenter Calibration", "", "code")] public int DecenterCalAFPos { get; set; } = 2048;
        [Condition("OIS Decenter Calibration", "X MG Offset Pos", "OIS Decenter Calibration", "", "code")] public int Decenter_X_MGOffset_Pos { get; set; } = 500;
        [Condition("OIS Decenter Calibration", "X MG Offset Neg", "OIS Decenter Calibration", "", "code")] public int Decenter_X_MGOffset_Neg { get; set; } = -500;
        [Condition("OIS Decenter Calibration", "Y MG Offset Pos", "OIS Decenter Calibration", "", "code")] public int Decenter_Y_MGOffset_Pos { get; set; } = 500;
        [Condition("OIS Decenter Calibration", "Y MG Offset Neg", "OIS Decenter Calibration", "", "code")] public int Decenter_Y_MGOffset_Neg { get; set; } = -500;
        [Condition("OIS Decenter Calibration", "Decenter Threshold", "OIS Decenter Calibration", "", "um")] public double DecenterThr { get; set; } = 0;

        [Condition("OIS Xtalk 2", "X Min Code", "OIS Xtalk 2", "", "code")] public int xTalk2MinCodeX { get; set; } = -2048;
        [Condition("OIS Xtalk 2", "X Max Code", "OIS Xtalk 2", "", "code")] public int xTalk2MaxCodeX { get; set; } = 2048;
        [Condition("OIS Xtalk 2", "Y Min Code", "OIS Xtalk 2", "", "code")] public int xTalk2MinCodeY { get; set; } = -2048;
        [Condition("OIS Xtalk 2", "Y Max Code", "OIS Xtalk 2", "", "code")] public int xTalk2MaxCodeY { get; set; } = 2048;
        [Condition("OIS Xtalk 2", "Delay", "OIS Xtalk 2", "", "ms")] public int Xtalk2Delay { get; set; } = 100;
        [Condition("OIS Xtalk 2", "AF Position", "OIS Xtalk 2", "", "code")] public int Xtalk2AFPos { get; set; } = 2048;


        [Condition("AF Fluctuation", "Min Code", "AF Fluctuation", "", "code")] public int AFFluctuationMinCode { get; set; } = 0;
        [Condition("AF Fluctuation", "Max Code", "AF Fluctuation", "", "code")] public int AFFluctuationMaxCode { get; set; } = 4095;
        [Condition("AF Fluctuation", "Step Code", "AF Fluctuation", "", "code")] public int AFFluctuationStepCode { get; set; } = 100;
        [Condition("AF Fluctuation", "Delay", "AF Fluctuation", "", "ms")] public int AFFluctuationDelay { get; set; } = 100;
        [Condition("AF Fluctuation", "Count", "AF Fluctuation", "", "dec")] public int AFFluctuationCount { get; set; } = 100;


        [Condition("OIS X Fluctuation", "Min Code", "OIS X Fluctuation", "", "code")] public int XFluctuationMinCode { get; set; } = -2048;
        [Condition("OIS X Fluctuation", "Max Code", "OIS X Fluctuation", "", "code")] public int XFluctuationMaxCode { get; set; } = 2048;
        [Condition("OIS X Fluctuation", "Step Code", "OIS X Fluctuation", "", "code")] public int XFluctuationStepCode { get; set; } = 100;
        [Condition("OIS X Fluctuation", "Delay", "OIS X Fluctuation", "", "ms")] public int XFluctuationDelay { get; set; } = 100;
        [Condition("OIS X Fluctuation", "Count", "OIS X Fluctuation", "", "dec")] public int XFluctuationCount { get; set; } = 100;
        [Condition("OIS X Fluctuation", "AF Position", "OIS X Fluctuation", "", "code")] public int XFluctuationAFPos { get; set; } = 2048;

        [Condition("OIS Y Fluctuation", "Min Code", "OIS Y Fluctuation", "", "code")] public int YFluctuationMinCode { get; set; } = -2048;
        [Condition("OIS Y Fluctuation", "Max Code", "OIS Y Fluctuation", "", "code")] public int YFluctuationMaxCode { get; set; } = 2048;
        [Condition("OIS Y Fluctuation", "Step Code", "OIS Y Fluctuation", "", "code")] public int YFluctuationStepCode { get; set; } = 100;
        [Condition("OIS Y Fluctuation", "Delay", "OIS Y Fluctuation", "", "ms")] public int YFluctuationDelay { get; set; } = 100;
        [Condition("OIS Y Fluctuation", "Count", "OIS Y Fluctuation", "", "dec")] public int YFluctuationCount { get; set; } = 100;
        [Condition("OIS Y Fluctuation", "AF Position", "OIS Y Fluctuation", "", "code")] public int YFluctuationAFPos { get; set; } = 2048;

        [Condition("Verify AF_OIS Xtalk", "INF Code", "Verify AF_OIS Xtalk", "", "code")] public int Verify_AFOIS_Xtalk_inf { get; set; } = 425;
        [Condition("Verify AF_OIS Xtalk", "MAC Code", "Verify AF_OIS Xtalk", "", "code")] public int Verify_AFOIS_Xtalk_mac { get; set; } = 3648;
        [Condition("Verify AF_OIS Xtalk", "Stroke", "Verify AF_OIS Xtalk", "", "um")] public int Verify_AFOIS_Xtalk_Stroke { get; set; } = 160;

        [Condition("I2C", "I2C Clock", "", "", "KHz")] public int iI2Cclock { get; set; } = 400;

        [Condition("AF Settling Time", "", "", "", "")] public SettlingRcp[] AFSettling { get; set; } = new SettlingRcp[50];
        public void InitAFSettling()
        {
            AFSettling = new SettlingRcp[50];
            for (int i = 0; i < 50; i++)
            {
                AFSettling[i] = new SettlingRcp();
            }
        }

    }
    public class RetryCount
    {
        public List<Retry> RetryOption = new List<Retry>();

    }
    public class Retry
    {
        public string InspName { get; set; }
        public int Count { get; set; }
    }

    public enum InspType
    {
        Normal, 
        OKNG,
        OnlyMin,
        OnlyMax,
        MintoMax,
    }

  
    public enum SpecItem
    {
        [Spec("AF> Mecha Stroke", "um", InspType.Normal, "AF HallCalibration")] AFMechaStroke,
        [Spec("AF> HallCalibration", "um", InspType.Normal, "AF HallCalibration")] AF_NonEPAStroke,
        [Spec("OIS FW Download", "", InspType.OKNG, "OIS FW Donwload")] OISFWDownload,
        [Spec("OIS AutoBoot", "", InspType.OKNG, "OIS AutoBoot")] OISAutoBoot,
        [Spec("XY> HallCalibration", "", InspType.OKNG, "OIS HallCalibration")] XYHallCalibration,
        //[Spec("XY> OIS IC Mount Error", "any", InspType.Normal, "OIS IC Mount Error")] OISIMERes,
        //[Spec("XY> OIS XYZ Temperature", "any", InspType.Normal, "OIS XYZ Temperature")] TempRes,
        [Spec("XY> OIS XYZ aging", "OK/NG", InspType.Normal, "OIS XYZ Aging")] XYZAging,
        [Spec("XY> LinearCompensation", "any", InspType.Normal, "OIS LinearityCompensation")] XYLinearComp,
        [Spec("XY> X Decenter", "um", InspType.Normal, "X/Y Servo Decenter")] x_ServoDecenter,
        [Spec("XY> Y Decenter", "um", InspType.Normal, "X/Y Servo Decenter")] y_ServoDecenter,
        //[Spec("XY> OIS X OpenLoop", "any", InspType.Normal, "OIS X/Y OpenLoop")] OLTestXResult,
        //[Spec("XY> OIS Y OpenLoop", "any", InspType.Normal, "OIS X/Y OpenLoop")] OLTestYResult,
        //[Spec("XY> OIS AutoTest", "any", InspType.Normal, "Auto Test")] AutoTestRes,

      //  [Spec("USER> OIS Sensitivity test", "No.", InspType.Normal, "OIS Sensitivity Test")] OISSensitivityTestRes,
        [Spec("USER> AF Aging", "any", InspType.OnlyMax, "AF Aging")] AFScanAging,

        [Spec("AF> Displacement Range", "um", InspType.Normal, "AF Scan")] AF_Ratedstroke,
        [Spec("AF> Displacement Min", "um", InspType.OnlyMax, "AF Scan")] AF_Backwardstroke,
        [Spec("AF> Displacement Max", "um", InspType.OnlyMin, "AF Scan")] AF_Forwardstroke,
        [Spec("AF> Hysteresis", "um", InspType.OnlyMax, "AF Scan")] AF_Hysteresis,
        [Spec("AF> Linearity(R)", "um", InspType.OnlyMax, "AF Scan")] AF_Linearity,
        [Spec("AF> Current", "mA", InspType.MintoMax, "AF Scan")] AF_Current,
        [Spec("AF> Tilt", "min", InspType.Normal, "AF Scan")] AF_Tilt,
        [Spec("AF> DynamicTilt(Regional)", "min", InspType.Normal, "AF Scan")] AF_DynamicTilt_Regional,
        [Spec("AF> Hall Shift Verify", "OK/NG", InspType.OKNG, "X/Y Drift Test")] HallShiftVerify,
     


        [Spec("AF Rated Stroke", "um", InspType.Normal, "AF Rated Stroke")] AF_Ratedstroke2,
        [Spec("AF Sensitivity", "um/code", InspType.Normal, "AF Rated Stroke")] AFSensitivity,

        [Spec("X> Displacement Range", "um", InspType.Normal, "OIS X Scan")] OISX_Ratedstroke,
        [Spec("X> Displacement Min", "um", InspType.OnlyMax, "OIS X Scan")] OISX_Backwardstroke,
        [Spec("X> Displacement Max", "um", InspType.OnlyMin, "OIS X Scan")] OISX_Forwardstroke,
        [Spec("X> Hysteresis", "um", InspType.OnlyMax, "OIS X Scan")] OISX_Hysteresis,      
        [Spec("X> Linearity(R)", "um", InspType.OnlyMax, "OIS X Scan")] OISX_Linearity,
        [Spec("X> Current", "mA", InspType.MintoMax, "OIS X Scan")] OISX_Current,
        [Spec("X> Xy crosstalk", "um", InspType.OnlyMax, "OIS X Scan")] OISX_xTalk,
        [Spec("X> Hall Decenter(Centering Error)", "um", InspType.Normal, "OIS X Scan")] x_HallDecenter,
        [Spec("X> Sensitivity", "um/code", InspType.Normal, "OIS X Scan")] x_Sensitivity,
        [Spec("X> Dynamic Tilt", "min", InspType.Normal, "OIS X Scan")] xDynamicTilt,

        [Spec("Y> Displacement Range", "um", InspType.Normal, "OIS Y Scan")] OISY_Ratedstroke,
        [Spec("Y> Displacement Min", "um", InspType.OnlyMax, "OIS Y Scan")] OISY_Backwardstroke,
        [Spec("Y> Displacement Max", "um", InspType.OnlyMin, "OIS Y Scan")] OISY_Forwardstroke,
        [Spec("Y> Hysteresis", "um", InspType.OnlyMax, "OIS Y Scan")] OISY_Hysteresis,
        [Spec("Y> Linearity(R)", "um", InspType.OnlyMax, "OIS Y Scan")] OISY_Linearity,
        [Spec("Y> Current", "mA", InspType.MintoMax, "OIS Y Scan")] OISY_Current,
        [Spec("Y> Yx crosstalk", "um", InspType.OnlyMax, "OIS Y Scan")] OISY_xTalk,
        [Spec("Y> Hall Decenter(Centering Error)", "um", InspType.Normal, "OIS Y Scan")] y_HallDecenter,
        [Spec("Y> Sensitivity", "um/code", InspType.Normal, "OIS Y Scan")] y_Sensitivity,
        [Spec("Y> Dynamic Tilt", "min", InspType.Normal, "OIS Y Scan")] yDynamicTilt,
        //[Spec("USER> X Through Peak 25", "dB", InspType.OnlyMax, "through Peak 25")] ThroughPeak_X_Gain,
        //[Spec("USER> Y Through Peak 25", "dB", InspType.OnlyMax, "through Peak 25")] ThroughPeak_Y_Gain,

        [Spec("OIS X Phase Margin", "deg", InspType.Normal, "OIS Phase Margin")] FRAX_PhaseMargin,
        [Spec("OIS Y Phase Margin", "deg", InspType.Normal, "OIS Phase Margin")] FRAY_PhaseMargin,
        [Spec("OIS X Gain Margin", "dB", InspType.Normal, "OIS Gain Margin")] FRAX_GainMargin,
        [Spec("OIS Y Gain Margin", "dB", InspType.Normal, "OIS Gain Margin")] FRAY_GainMargin,

        [Spec("OIS X Loop Gain", "dB", InspType.Normal, "OIS LoopGain")] FRAX_LoopGain,
        [Spec("OIS Y Loop Gain", "dB", InspType.Normal, "OIS LoopGain")] FRAY_LoopGain,

        [Spec("AF Gain Margin", "dB", InspType.Normal, "AF Gain Margin")] FRAAF_GainMargin,
        [Spec("AF Phase Margin", "deg", InspType.Normal, "AF Phase Margin")] FRAAF_PhaseMargin,
        [Spec("AF Loop Gain", "dB", InspType.Normal, "AF LoopGain")] AF_LoopGain,
        //   [Spec("USER> AF -4dB Phase Margin", "deg", InspType.Normal, "AF Phase Margin")] FRAAF_4dB_PhaseMargin,
        [Spec("AF OIS XTalk Calibration", "um", InspType.OnlyMax, "AF OIS XTalk Calibration")] xTaklMaxDiff,
        [Spec("OIS Linear/Crosstalk Calibration", "", InspType.OKNG, "OIS Linear/Crosstalk Calibration")] OISLCCComp,

        [Spec("X Decenter Cal", "code", InspType.Normal, "OIS Decenter Calibration")] OISXDecenterCal,
        [Spec("Y Decenter Cal", "code", InspType.Normal, "OIS Decenter Calibration")] OISYDecenterCal,
       
        [Spec("AF HAT Diff(M-m)", "code", InspType.OnlyMax, "AF HAT")] AFHAT_Diff,
        [Spec("AF HAT Diff(Max Error)", "code", InspType.OnlyMax, "AF HAT")] AFHAT_Diff_MaxError,
        [Spec("OIS X HAT Diff(M-m)", "code", InspType.OnlyMax, "OIS HAT")] OISXHAT_Diff,
        [Spec("OIS X HAT Diff(Max Error)", "code", InspType.OnlyMax, "OIS HAT")] OISXHAT_Diff_MaxError,
        [Spec("OIS Y HAT Diff(M-m)", "code", InspType.OnlyMax, "OIS HAT")] OISYHAT_Diff,
        [Spec("OIS Y HAT Diff(Max Error)", "code", InspType.OnlyMax, "OIS HAT")] OISYHAT_Diff_MaxError,

        [Spec("AF RunUp 50ms Diff", "code", InspType.OnlyMax, "AF RunUp Test")] AF_Diff_50ms,
        [Spec("AF RunUp 100ms Diff", "code", InspType.OnlyMax, "AF RunUp Test")] AF_Diff_100ms,
        [Spec("AF RunUp 150ms Diff", "code", InspType.OnlyMax, "AF RunUp Test")] AF_Diff_150ms,

        [Spec("OIS X RunUp Diff", "code", InspType.OnlyMax, "OIS RunUp Test")] OISX_Diff_ms,
        //[Spec("OIS X RunUp 100ms Diff", "code", InspType.OnlyMax, "OIS RunUp Test")] OISX_Diff_100ms,
        //[Spec("OIS X RunUp 150ms Diff", "code", InspType.OnlyMax, "OIS RunUp Test")] OISX_Diff_150ms,

        [Spec("OIS Y RunUp Diff", "code", InspType.OnlyMax, "OIS RunUp Test")] OISY_Diff_ms,
        //[Spec("OIS Y RunUp 100ms Diff", "code", InspType.OnlyMax, "OIS RunUp Test")] OISY_Diff_100ms,
        //[Spec("OIS Y RunUp 150ms Diff", "code", InspType.OnlyMax, "OIS RunUp Test")] OISY_Diff_150ms,

        [Spec("Xy Crosstalk2", "%", InspType.OnlyMax, "OIS Xtalk 2")] XyCrosstalk2,
        [Spec("Yx Crosstalk2", "%", InspType.OnlyMax, "OIS Xtalk 2")] YxCrosstalk2,

        [Spec("AF Fluctuation", "um", InspType.Normal, "AF Fluctuation")] AFfluctuation,
        [Spec("OIS X Fluctuation", "um", InspType.Normal, "OIS X Fluctuation")] Xfluctuation,
        [Spec("OIS Y Fluctuation", "um", InspType.Normal, "OIS Y Fluctuation")] Yfluctuation,


        [Spec("AF> Stabilize Time 1", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime1,
        [Spec("AF> Stabilize Time 2", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime2,
        [Spec("AF> Stabilize Time 3", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime3,
        [Spec("AF> Stabilize Time 4", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime4,
        [Spec("AF> Stabilize Time 5", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime5,
        [Spec("AF> Stabilize Time 6", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime6,
        [Spec("AF> Stabilize Time 7", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime7,
        [Spec("AF> Stabilize Time 8", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime8,
        [Spec("AF> Stabilize Time 9", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime9,
        [Spec("AF> Stabilize Time 10", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime10,
        [Spec("AF> Stabilize Time 11", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime11,
        [Spec("AF> Stabilize Time 12", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime12,
        [Spec("AF> Stabilize Time 13", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime13,
        [Spec("AF> Stabilize Time 14", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime14,
        [Spec("AF> Stabilize Time 15", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime15,
        [Spec("AF> Stabilize Time 16", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime16,
        [Spec("AF> Stabilize Time 17", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime17,
        [Spec("AF> Stabilize Time 18", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime18,
        [Spec("AF> Stabilize Time 19", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime19,
        [Spec("AF> Stabilize Time 20", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime20,
        [Spec("AF> Stabilize Time 21", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime21,
        [Spec("AF> Stabilize Time 22", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime22,
        [Spec("AF> Stabilize Time 23", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime23,
        [Spec("AF> Stabilize Time 24", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime24,
        [Spec("AF> Stabilize Time 25", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime25,
        [Spec("AF> Stabilize Time 26", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime26,
        [Spec("AF> Stabilize Time 27", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime27,
        [Spec("AF> Stabilize Time 28", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime28,
        [Spec("AF> Stabilize Time 29", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime29,
        [Spec("AF> Stabilize Time 30", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime30,
        [Spec("AF> Stabilize Time 31", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime31,
        [Spec("AF> Stabilize Time 32", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime32,
        [Spec("AF> Stabilize Time 33", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime33,
        [Spec("AF> Stabilize Time 34", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime34,
        [Spec("AF> Stabilize Time 35", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime35,
        [Spec("AF> Stabilize Time 36", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime36,
        [Spec("AF> Stabilize Time 37", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime37,
        [Spec("AF> Stabilize Time 38", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime38,
        [Spec("AF> Stabilize Time 39", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime39,
        [Spec("AF> Stabilize Time 40", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime40,
        [Spec("AF> Stabilize Time 41", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime41,
        [Spec("AF> Stabilize Time 42", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime42,
        [Spec("AF> Stabilize Time 43", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime43,
        [Spec("AF> Stabilize Time 44", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime44,
        [Spec("AF> Stabilize Time 45", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime45,
        [Spec("AF> Stabilize Time 46", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime46,
        [Spec("AF> Stabilize Time 47", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime47,
        [Spec("AF> Stabilize Time 48", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime48,
        [Spec("AF> Stabilize Time 49", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime49,
        [Spec("AF> Stabilize Time 50", "ms", InspType.OnlyMax, "AF Settling")] AF_SettillingTime50,
      


        [Spec("AF PID Verify", "any", InspType.Normal, "AF PID Verify")] AFPIDVerifyRes,
      //  [Spec("USER> OIS PID Verify", "any", InspType.Normal, "OIS PID Verify")] OISPIDVerifyRes,

        Length,

    };
   
    public class Spec
    {
        public List<SpecArray> specList { get; set; } = new List<SpecArray>();
        public void InitSpecList()
        {
            specList.Clear();
            for (int i = 0; i < (int)SpecItem.Length; i++)
            {
                SpecItem s = (SpecItem)i;
                specList.Add(new SpecArray());
              
                specList[i].Unit = DataIO.GetEnumArttribute<SpecAttribute>(s)?.Unit;
                specList[i].DisplayName = DataIO.GetEnumArttribute<SpecAttribute>(s)?.DisplayName;
                specList[i].InspectionType = (InspType)DataIO.GetEnumArttribute<SpecAttribute>(s)?.InspType;
                specList[i].Category = DataIO.GetEnumArttribute<SpecAttribute>(s)?.Category;
            }
        }

    }

    public class SpecArray
    {
        public double MinSpec { get; set; } = 0;
        public double MaxSpec { get; set; } = 0;
        public bool OnOff { get; set; } = true;
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string Unit { get; set; }
        public int FailCnt { get; set; }

        public InspType InspectionType { get; set; }
    }

    public class TotalYield
    {
        public int LastSampleNum { get; set; }
        public int TotlaTested { get; set; }
        public int TotlaPassed { get; set; }
        public int TotlaFailed { get; set; }

    }
    public class ResultItems
    {
        public double Val = double.MaxValue;
        public bool bPass = true;
        public string msg = "";
    }
    public class PassFail
    {
        public int FirstFailIndex;
        public string FirstFail;    
        public string TotalTime;
        public List<ResultItems> Results = new List<ResultItems>();
    }

  
    public class CurrentPath
    {

        public string ConditionName { get; set; } = "";
        public string SpecName { get; set; } = "";
        public string AFPidPath { get; set; } = "";
        public string OISXPidPath { get; set; } = "";
        public string OISYPidPath { get; set; } = "";

        public string OISFWPath { get; set; } = "";
        public string OISBaseCalPath { get; set; } = "";

    }
    public class Model : BaseRecipe
    {
        public string MCNum;
        public string TesterNo;
        
        public string MCType;
        private string lotID;
        public string LotID
        {
            get { return lotID; }
            set
            {
                if (value != lotID)
                { lotID = value; IsLotChanged = true; }
                else IsLotChanged = false;
            }
        }
        public string OperatorName;

        public List<string> List = new List<string>();

        public List<string> MakerList = new List<string>();
      
      
        public List<string> SupplierList = new List<string>();
        public List<string> MCTypeList = new List<string>();


        public bool IsLotChanged = false;
        public event EventHandler Changed = null;

        public Model()
        {
            FilePath = STATIC.RootDir + "Model.txt";

            MCTypeList.Add("Normal");
            MCTypeList.Add("Master");
            MCTypeList.Add("Slave");
            MCTypeList.Add("Handler");
            MCTypeList.Add("Posture_M");
            MCTypeList.Add("Posture_S");

            Read();
        }
        public override void Read(string filePath = "")
        {
            base.Read();
            if (!File.Exists(FilePath))
            {
                List.Add("0");
                List.Add("0");
                List.Add("Normal");
               
                STATIC.SetTextLine(FilePath, List);
                SetParam();
            }
            else
            {
                List = STATIC.GetTextAll(FilePath);
                SetParam();
            }
        }
        public override void Save(string filePath = "")
        {
            List.Clear();
            List.Add(MCNum);
            List.Add(TesterNo);
            List.Add(MCType);


            STATIC.SetTextLine(FilePath, List);
        }

        public override void SetParam()
        {
            base.SetParam();
            int index = 0;
            MCNum = List[index++];
            TesterNo = List[index++];
            MCType = List[index++];

        }
        public void LotChanged()
        {
            Changed?.Invoke(null, EventArgs.Empty);
        }
    }
    public class VisionFile
    {
        public int RawGain { get; set; } = 40;
        public double Gamma { get; set; } = 0.85;
        public int Exposure { get; set; } = 73;
        public int EdgeBand { get; set; } = 9;
        public double LEDCurrentL { get; set; } = 2.05;
        public double LEDCurrentR { get; set; } = 1.9;
    }
   

    public class NewYield
    {
        public string ItemName { get; set; }
        public int FailCnt { get; set; }
    }

    public class Password
    {
        public string PW { get; set; } = "0";
    }

    public class SettlingRcp
    {
        public int StartCode { get; set; } = 0;
        public int EndCode { get; set; } = 4095;
        public double Criteria { get; set; } = 5;
        public int TryCount { get; set; } = 1;
        public bool UseFlag { get; set; } = false;
    }

  

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OptionAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public OptionAttribute(string des)
        {
            DisplayName = des;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ConditionAttribute : Attribute
    {
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string ToDo1 { get; set; }
        public string ToDo2 { get; set; }
        public string Unit { get; set; }
        public ConditionAttribute(string des, string des2, string des3, string des4, string des5)
        {
            Category = des;
            DisplayName = des2;
            ToDo1 = des3;
            ToDo2 = des4;
            Unit = des5;

        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SpecAttribute : Attribute
    {
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string Unit { get; set; }
        public InspType InspType { get; set; }
        public SpecAttribute(string des, string des2, InspType type, string des3)
        {
            Category = des3;
            DisplayName = des;
            Unit = des2;
            InspType = type;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CommonAttribute : Attribute
    {
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public CommonAttribute(string des, string des2)
        {
            Category = des;
            DisplayName = des2;
        }
    }

}

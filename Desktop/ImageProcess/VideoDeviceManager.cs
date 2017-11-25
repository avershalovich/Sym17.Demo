using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;

namespace ImageProcess
{
    public class VideoDevice
    {
        public string DeviceName;
        public int DeviceId;
        public Guid Identifier;

        public VideoDevice(int id, string name, Guid identity = new Guid())
        {
            DeviceId = id;
            DeviceName = name;
            Identifier = identity;
        }

        /// <summary>
        /// Represent the Device as a String
        /// </summary>
        /// <returns>The string representation of this color</returns>
        public override string ToString()
        {
            return string.Format("[{0}] {1}", DeviceId, DeviceName);
        }
    }
    public static class VideoDeviceManager
    {
        public static DsDevice[] GetVideoDevices()
        {
            return DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        }
        public static List<BitmapInfoHeader> GetAllAvailableResolution(DsDevice vidDev)
        {
            try
            {
                int hr, bitCount = 0;

                IBaseFilter sourceFilter = null;

                var m_FilterGraph2 = new FilterGraph() as IFilterGraph2;
                hr = m_FilterGraph2.AddSourceFilterForMoniker(vidDev.Mon, null, vidDev.Name, out sourceFilter);
                var pRaw2 = DsFindPin.ByCategory(sourceFilter, PinCategory.Capture, 0);
                var AvailableResolutions = new List<BitmapInfoHeader>();

                VideoInfoHeader v = new VideoInfoHeader();
                IEnumMediaTypes mediaTypeEnum;
                hr = pRaw2.EnumMediaTypes(out mediaTypeEnum);

                AMMediaType[] mediaTypes = new AMMediaType[1];
                IntPtr fetched = IntPtr.Zero;
                hr = mediaTypeEnum.Next(1, mediaTypes, fetched);

                while (fetched != null && mediaTypes[0] != null)
                {
                    Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                    if (v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                    {
                        if (v.BmiHeader.BitCount > bitCount)
                        {
                            AvailableResolutions.Clear();
                            bitCount = v.BmiHeader.BitCount;
                        }
                        AvailableResolutions.Add(v.BmiHeader);
                    }
                    hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
                }
                return AvailableResolutions;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<BitmapInfoHeader>();
            }
        }

    }


}

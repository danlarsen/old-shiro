using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Shiro.Interop;

namespace Shiro.Libraries
{
    public struct StreamComb
    {
        public StreamWriter writer;
        public StreamReader reader;
    }

    [ShiroClass("File", KeepClassLoaded=true)]
    public class File
    {
        protected Dictionary<int, StreamComb> files = new Dictionary<int, StreamComb>();
        protected int LastIndex = 1;

		[ShiroMethod("fileExists", 1)]
		public string fileExists(string fName)
		{
			return System.IO.File.Exists(fName).ToString();
		}
		
		[ShiroMethod("openr", 1)]
        public string OpenR(string fName)
        {
            StreamComb st = new StreamComb();

            try
            {
                st.reader = new StreamReader(fName);
                st.writer = null;
                files.Add(LastIndex++, st);
            }
            catch (Exception)
            {
                throw new ShiroException("File not found");
            }

            return (LastIndex - 1).ToString();
        }

        [ShiroMethod("openw", 2)]
        public string OpenW(string fName, string append)
        {
            StreamComb st = new StreamComb();

            try
            {
                st.reader = null;
                st.writer = new StreamWriter(fName, (append == "true"));
                files.Add(LastIndex++, st);
            }
            catch (Exception)
            {
                throw new ShiroException("File not found");
            }

            return (LastIndex - 1).ToString();
        }

        [ShiroMethod("write", 2)]
        public string Write(int handle, string value)
        {
            if (!files.ContainsKey(handle))
                throw new ShiroException("Unknown File Handle");
            if (files[handle].writer == null)
                throw new ShiroException("File is open in Read mode");

            files[handle].writer.Write(value);
            return value;
        }
        [ShiroMethod("writeln", 2)]
        public string WriteLn(int hndl, string value)
        {
            return Write(hndl, value + Environment.NewLine);
        }

        [ShiroMethod("readln", 1)]
		public string ReadLn(int handle)
        {
            if (!files.ContainsKey(handle))
                throw new ShiroException("Unknown File Handle");
            if (files[handle].reader == null)
                throw new ShiroException("File is open in Write mode");

            return files[handle].reader.ReadLine();
        }
        [ShiroMethod("read", 1)]
        public string read(int handle)
        {
            if (!files.ContainsKey(handle))
                throw new ShiroException("Unknown File Handle");
            if (files[handle].reader == null)
                throw new ShiroException("File is open in Write mode");

            return files[handle].reader.ReadToEnd();
        }

        [ShiroMethod("eof", 1)]
        public string EoF(int handle)
        {
            if (!files.ContainsKey(handle))
                throw new ShiroException("Unknown File Handle");
            if (files[handle].reader == null)
                throw new ShiroException("File is open in Write mode");

            return files[handle].reader.EndOfStream ? "true" : "false";
        }

        [ShiroMethod("close", 1)]
		public string Close(int index)
        {
            if(files.ContainsKey(index))
            {
                if (null == files[index].reader)
                {
                    files[index].writer.Flush();
                    files[index].writer.Close();
                }
                else
                {
                    files[index].reader.Close();
                }
                
                files.Remove(index);
                return "true";
            }
            return "false";
        }
    }
}

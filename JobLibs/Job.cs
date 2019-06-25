namespace JobLib.Struct
{
    using System;

    /// <summary>
    /// this struct storing a job info 
    /// </summary>
    public struct Job
    {
        public string Message { get; set; }
        public Char JobName { get; set; }

        public override string ToString()
        {
            return Message;
        }

        public override bool Equals(object ob)
        {
            if (ob is Job)
            {
                Job c = (Job)ob;
                return JobName == c.JobName && Message == c.Message;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return JobName.GetHashCode() ^ Message.GetHashCode();
        }

    }
  

}

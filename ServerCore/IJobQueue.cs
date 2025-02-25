using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
	public interface IJobQueue
	{
		void Push(Action job);
	}
}

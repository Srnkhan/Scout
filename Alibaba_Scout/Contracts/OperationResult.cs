using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alibaba_Scout.Contracts
{
    internal class OperationResult
    {
        public string Result { get; private set; }
        public bool IsSuccess { get; private set; }

        private static OperationResult _instance;

        public static OperationResult SuccessResult(string result)
        {
            if (_instance == null)
            {
                _instance = new OperationResult(result,true);
            }
            return _instance;
        }
        public static OperationResult ErrorResult(string result)
        {
            if (_instance == null)
            {
                _instance = new OperationResult(result, false);
            }
            return _instance;
        }
        private  OperationResult(string result, bool ısSuccess)
        {
            Result = result;
            IsSuccess = ısSuccess;
        }
        public OperationResult()
        {

        }
     
    }
}

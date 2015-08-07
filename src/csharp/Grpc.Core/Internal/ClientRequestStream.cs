#region Copyright notice and license
// Copyright 2015, Google Inc.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
// 
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Threading.Tasks;
using Grpc.Core.Internal;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Writes requests asynchronously to an underlying AsyncCall object.
    /// </summary>
    internal class ClientRequestStream<TRequest, TResponse> : IClientStreamWriter<TRequest>
    {
        readonly AsyncCall<TRequest, TResponse> call;
        WriteOptions writeOptions;

        public ClientRequestStream(AsyncCall<TRequest, TResponse> call)
        {
            this.call = call;
            this.writeOptions = call.Details.Options.WriteOptions;
        }

        public Task WriteAsync(TRequest message)
        {
            var taskSource = new AsyncCompletionTaskSource<object>();
            call.StartSendMessage(message, GetWriteFlags(), taskSource.CompletionDelegate);
            return taskSource.Task;
        }

        public Task CompleteAsync()
        {
            var taskSource = new AsyncCompletionTaskSource<object>();
            call.StartSendCloseFromClient(GetWriteFlags(), taskSource.CompletionDelegate);
            return taskSource.Task;
        }

        public WriteOptions WriteOptions
        {
            get
            {
                return this.writeOptions;
            }

            set
            {
                writeOptions = value;
            }
        }

        private WriteFlags GetWriteFlags()
        {
            var options = writeOptions;
            return options != null ? options.Flags : default(WriteFlags);
        }
    }
}

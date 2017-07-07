/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-11-30
 */
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// Represents a view model serializer instance
    /// </summary>
    public interface IViewModelSerializer
    {

        /// <summary>
        /// Gets or sets the view model definition of the view model serlializer
        /// </summary>
        ViewModelDescription ViewModel { get; set; }

        /// <summary>
        /// Loads all instance of serializers (pre-compiled) from the specified assembly
        /// </summary>
        void LoadSerializerAssembly(Assembly asm);

        /// <summary>
        /// Serializes the object <paramref name="data"/> onto stream <paramref name="s"/>
        /// </summary>
        void Serialize(Stream s, IdentifiedData data);
        
        /// <summary>
        /// Serializes the object <paramref name="data"/> onto stream <paramref name="s"/>
        /// </summary>
        void Serialize(TextWriter s, IdentifiedData data);

        /// <summary>
        /// De-serializes the specified object from the stream
        /// </summary>
        TModel DeSerialize<TModel>(Stream s);

        /// <summary>
        /// De-serialize the specified stream to type
        /// </summary>
        Object DeSerialize(Stream s, Type t);

        /// <summary>
        /// Loads the associations for the specified object
        /// </summary>
        IEnumerable<TAssociation> LoadCollection<TAssociation>(Guid sourceKey) where TAssociation : IdentifiedData, ISimpleAssociation, new();

        /// <summary>
        /// Loads the specified related object
        /// </summary>
        TRelated LoadRelated<TRelated>(Guid? objectKey) where TRelated : IdentifiedData, new();
        
    }
}

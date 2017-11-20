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
 * Date: 2016-8-15
 */
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a repository which deals with metadata such as assigning authorities,
	/// concept classes, etc.
	/// </summary>
	public interface IMetadataRepositoryService
	{
		/// <summary>
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		CodeSystem CreateCodeSystem(CodeSystem codeSystem);

		/// <summary>
		/// Deletes the code system.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the deleted code system.</returns>
		CodeSystem DeleteCodeSystem(Guid key);

		/// <summary>
		/// Creates the type of the extension.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the created extension type.s</returns>
		ExtensionType CreateExtensionType(ExtensionType extensionType);

		/// <summary>
		/// Deletes the type of the extension.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the deleted extension type.</returns>
		ExtensionType DeleteExtensionType(Guid id);

		/// <summary>
		/// Finds the specified assigning authority
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of assigning authorities.</returns>
		IEnumerable<AssigningAuthority> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> expression);

		/// <summary>
		/// Finds the specified assigning authority with restrictions
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalCount">The total count.</param>
		/// <returns>Returns a list of assigning authorities.</returns>
		IEnumerable<AssigningAuthority> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> expression, int offset, int count, out int totalCount);

		/// <summary>
		/// Finds the code system.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of code systems which match the given expression.</returns>
		IEnumerable<CodeSystem> FindCodeSystem(Expression<Func<CodeSystem, bool>> expression);

		/// <summary>
		/// Finds the code system.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalCount">The total count.</param>
		/// <returns>Returns a list of code systems which match the given expression.</returns>
		IEnumerable<CodeSystem> FindCodeSystem(Expression<Func<CodeSystem, bool>> expression, int offset, int? count, out int totalCount);

		/// <summary>
		/// Finds an extension type for a specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of extension types.</returns>
		IEnumerable<ExtensionType> FindExtensionType(Expression<Func<ExtensionType, bool>> expression);

		/// <summary>
		/// Finds an extension type for a specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalCount">The total count.</param>
		/// <returns>Returns a list of extension types.</returns>
		IEnumerable<ExtensionType> FindExtensionType(Expression<Func<ExtensionType, bool>> expression, int offset, int? count, out int totalCount);

		/// <summary>
		/// Find template definitions matching the query
		/// </summary>
		IEnumerable<TemplateDefinition> FindTemplateDefinitions(Expression<Func<TemplateDefinition, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets an assigning authority
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns an assigning authority.</returns>
		AssigningAuthority GetAssigningAuthority(Guid id);

		/// <summary>
		/// Get assigning authority from Uri value
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Returns an assigning authority.</returns>
		AssigningAuthority GetAssigningAuthority(Uri value);

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the code system, or null if no code system is found.</returns>
		CodeSystem GetCodeSystem(Guid id);

		/// <summary>
		/// Gets the extension type.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns an extension type or null of no extension type is found.</returns>
		ExtensionType GetExtensionType(Guid id);

		/// <summary>
		/// Gets the extension type.
		/// </summary>
		/// <param name="value">The URI of the extension.</param>
		/// <returns>Returns an extension type or null of no extension type is found.</returns>
		ExtensionType GetExtensionType(Uri value);

		/// <summary>
		/// Updates the type of the extension.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the updated extension type.</returns>
		ExtensionType UpdateExtensionType(ExtensionType extensionType);

		/// <summary>
		/// Updates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the updated code system.</returns>
		CodeSystem UpdateCodeSystem(CodeSystem codeSystem);

        /// <summary>
        /// Get tempate definition
        /// </summary>
        TemplateDefinition GetTemplateDefinition(string mnemonic);

        /// <summary>
        /// Create a template definition
        /// </summary>
        TemplateDefinition CreateTemplateDefinition(TemplateDefinition templateDefinition);
    }
}
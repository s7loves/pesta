/* Copyright (c) 2008 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



/**
 * An object representing a person in an OpenSocial environment; extends
 * OpenSocialObject class by adding convenience methods for easily
 * accessing special fields unique to instances of this class.
 *
 * @author Jason Cooper
 */
using System;
using System.Text;

public class OpenSocialPerson : OpenSocialObject {

  public OpenSocialPerson()
  {
  }

  /**
   * Retrieves the OpenSocial ID associated with the instance. Returns an
   * empty string if no ID has been set.
   */
  public String getId() {
    OpenSocialField IDField = this.getField("id");

    if (IDField != null && !IDField.isComplex()) {
      return IDField.getStringValue();
    }

    return "";
  }

  /**
   * Retrieves the display name (typically given name followed by family name)
   * associated with the instance. Returns an empty string if no name has been
   * set.
   * 
   * @throws OpenSocialException 
   */
  public String getDisplayName() 
  {
    OpenSocialField nicknameField = this.getField("nickname");
    OpenSocialField nameField = this.getField("name");
    StringBuilder name = new StringBuilder();

    if (nameField != null) {
      if (nameField.isComplex()) {
        OpenSocialObject nameObject = nameField.getValue();

        if (nameObject.hasField("givenName")) {
          name.Append(nameObject.getField("givenName").getStringValue());
        }

        if (nameObject.hasField("givenName")
               && nameObject.hasField("familyName")) {
          name.Append(" ");
        }

        if (nameObject.hasField("familyName")) {
          name.Append(nameObject.getField("familyName").getStringValue());
        }
      } else {
        name.Append(nameField.getStringValue());
      }
    } else if (nicknameField != null) {
      if (!nicknameField.isComplex()) {
        name.Append(nicknameField.getStringValue());
      }
    }

    return name.ToString();
  }
}

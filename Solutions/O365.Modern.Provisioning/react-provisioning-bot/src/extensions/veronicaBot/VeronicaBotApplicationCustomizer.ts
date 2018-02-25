import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
  BaseApplicationCustomizer, PlaceholderContent, PlaceholderName
} from '@microsoft/sp-application-base';
import { Dialog } from '@microsoft/sp-dialog';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import IGraphBotProps from './components/IGraphBotProps';
import GraphBot from './components/GraphBot';
import * as strings from 'VeronicaBotApplicationCustomizerStrings';

const LOG_SOURCE: string = 'VeronicaBotApplicationCustomizer';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface IVeronicaBotApplicationCustomizerProperties {
  // This is an example; replace with your own property
  testMessage: string;
}

/** A Custom Action which can be run during execution of a Client Side Application */
export default class VeronicaBotApplicationCustomizer
  extends BaseApplicationCustomizer<IVeronicaBotApplicationCustomizerProperties> {

  private _topPlaceHolder: PlaceholderContent;

  @override
  public onInit(): Promise<void> {
    Log.info(LOG_SOURCE, `Initialized ${strings.Title}`);

    let message: string = this.properties.testMessage;
    if (!message) {
      message = '(No properties were provided.)';
    }

    //Dialog.alert(`Hello from ${strings.Title}:\n\n${message}`);
    this._renderPlaceHolders();

    return Promise.resolve();
  }

  private _renderPlaceHolders(): void {

    // Check if the header placeholder is already set and if the header placeholder is available
    if (!this._topPlaceHolder && this.context.placeholderProvider.placeholderNames.indexOf(PlaceholderName.Top) !== -1) {
      this._topPlaceHolder = this.context.placeholderProvider.tryCreateContent(PlaceholderName.Top, {
        onDispose: () => {}
      });

      // The extension should not assume that the expected placeholder is available.
      if (!this._topPlaceHolder) {
        console.error('The expected placeholder was not found.');
        return;
      }

      if (this._topPlaceHolder.domElement) {
        const element: React.ReactElement<IGraphBotProps> = React.createElement(
          GraphBot,
          {
            context: this.context
          } as IGraphBotProps
        );

        ReactDOM.render(element, this._topPlaceHolder.domElement);
      }
    }
  }
}

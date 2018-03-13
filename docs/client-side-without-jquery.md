# Client Side without jQuery (Angular, etc.)

## Installation

We provide an [individual npm package](https://www.npmjs.com/package/devextreme-aspnet-data-nojquery) to be used with client-side frameworks that do not depend on jQuery, for example, with Angular. Run the following command to install it:

    npm install devextreme-aspnet-data-nojquery

## API Reference and Usage

The API consists of a single method, `createStore`, which is described [here](client-side-with-jquery.md#api-reference). The following example shows how to use this method in an Angular app to create a store for the **DataGrid** widget. Note that the widget requires the [devextreme-angular](https://github.com/DevExpress/devextreme-angular#add-to-existing-app) package in addition.

**app.component.ts**

```TypeScript
import { Component } from '@angular/core';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';

@Component({
    selector: 'app-root',
    templateUrl: `
        <dx-data-grid
            [dataSource]="store">
        </dx-data-grid>
    `
})
export class AppComponent {
    store: CustomStore;
    constructor() {
        let serviceUrl = "http://url/to/my/service";
        this.store = createStore({
            key: "ID",
            loadUrl: serviceUrl + "/GetAction",
            insertUrl: serviceUrl + "/InsertAction",
            updateUrl: serviceUrl + "/UpdateAction",
            deleteUrl: serviceUrl + "/DeleteAction"
        });
    }
}
```

**app.module.ts**

```TypeScript
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { DxDataGridModule } from 'devextreme-angular';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    DxDataGridModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```
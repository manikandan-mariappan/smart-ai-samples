
import { enableRipple } from '@syncfusion/ej2-base';
import { getAzureChatAIRequest } from '../../ai-models';
import { pivotData } from './data-source';
import { FieldList, CalculatedField, Toolbar, ConditionalFormatting, NumberFormatting, PivotView, ToolbarArgs, IDataOptions, IDataSet } from '@syncfusion/ej2/pivotview';
import { createSpinner, Dialog, hideSpinner, showSpinner } from '@syncfusion/ej2/popups';
import { DropDownList, MultiSelect } from '@syncfusion/ej2/dropdowns';
import { Button, ChipList } from '@syncfusion/ej2/buttons';
import { TextBox } from '@syncfusion/ej2/inputs';
import { ItemModel } from '@syncfusion/ej2/navigations';
enableRipple(true);

let dropdownData: string[] = ['2025', '2026', '2027', '2028', '2029'];
let description: string;
let dataSourceSettings: IDataOptions = {
  enableSorting: true,
  columns: [{ name: 'Year' }, { name: 'Quarter' }],
  values: [{ name: 'Sold', caption: 'Units Sold' }, { name: 'Amount', caption: 'Sold Amount' }],
  dataSource: pivotData as IDataSet[],
  rows: [{ name: 'Country',expandAll: true }, { name: 'Products' }],
  formatSettings: [{ name: 'Amount', format: 'C0' }],
  filterSettings: [{name: 'Products', type: 'Include', items: ['Bikes', 'Road Bikes', 'Helmets', 'Bottles and Cages']}]
};
let chip: ChipList;
PivotView.Inject(FieldList, CalculatedField, Toolbar, ConditionalFormatting, NumberFormatting);
let pivotObj: PivotView = new PivotView({
  dataSourceSettings: {
    enableSorting: true,
    columns: [{ name: 'Year' }, { name: 'Quarter' }],
    values: [{ name: 'Sold', caption: 'Units Sold', type: 'Count' }, { name: 'Amount', caption: 'Sold Amount', type: 'Min' }],
    dataSource: pivotData as IDataSet[],
    rows: [{ name: 'Country', expandAll: true }, { name: 'Products' }],
    filterSettings: [{name: 'Products', type: 'Include', items: ['Bikes', 'Road Bikes', 'Helmets', 'Bottles and Cages']}]
  },
  width: '100%',
  height: 500,
  cellTemplate: '${getCellContent(data)}',
  toolbarRender: function (args: ToolbarArgs): void {
    (args.customToolbar as ItemModel[]).splice(5, 0, {
      type: 'Separator' 
    });
    (args.customToolbar as ItemModel[]).splice(6, 0, {
      text: 'AI Assist', tooltipText: 'AI Assist',
      prefixIcon: 'e-btn-icon e-icons e-assist-chat e-icon-left',
      click: toolbarClicked.bind(this)
    });
  },
  displayOption: { view: 'Both' },
  chartSettings: {
    value: 'Amount', enableExport: true, chartSeries: { type: 'Column', animation: { enable: false } }, enableMultipleAxis: false,
  },
  toolbar: ['Grid', 'Chart', 'SubTotal', 'GrandTotal', 'ConditionalFormatting', 'FieldList'],
  allowConditionalFormatting: true,
  allowPdfExport: true,
  showToolbar: true,
  allowCalculatedField: true,
  showFieldList: true
});
pivotObj.appendTo('#pivotTable');

(window as any).getCellContent = function (e: any) {
  let template: string;
  
  if (e && e.targetCell.className.indexOf('e-valuescontent') > -1) {
    let year = e.cellInfo.columnHeaders.replace(/^FY\s*/, '');
    if(dropdownData.includes(year)) {
      e.targetCell.classList.add('e-custom-class');
      template = '';
    } else {
      template = '';
  ``}
  } else {
      template = '';
  }
  return template;
};

let dialogContentDiv: HTMLElement = document.createElement('div');
dialogContentDiv.id = 'dialogContent';
let dialog: Dialog = new Dialog({
  minHeight: '200px',
  isModal: true,
  showCloseIcon: true,
  visible: false,
  header: '<span class="e-btn-icon e-icons e-assist-chat e-icon-left"></span> AI Assist',
  content: dialogContentDiv,
  buttons: [{
      click: onSubmit,
      buttonModel: { content: 'Submit', isPrimary: true }
  }],
  target: 'body'
});
dialog.appendTo('#pivotDialog');
createDialogContent(dialogContentDiv);

function createDialogContent(container: HTMLElement) {
  let categoryTitle1: HTMLElement = document.createElement('p');
  categoryTitle1.className = 'category-title';
  categoryTitle1.innerText = 'Pick a Suggested Query:';
  container.appendChild(categoryTitle1);

  let chipContainer: HTMLElement = document.createElement('div');
  chipContainer.className = 'chip-container';
  chip = new ChipList({
      chips: [
          { text: 'Predictive Analytics & Modeling' },
          { text: 'Intelligent Report Aggregation' },
          { text: 'Adaptive Filter Suggestions' }
      ],
      selection: 'Single',
      selectedChips: [0],
      click: onChipSelectionChange
  });
  chip.appendTo(chipContainer);
  container.appendChild(chipContainer);

  // Prompt section
  let categoryTitle2: HTMLElement = document.createElement('p');
  categoryTitle2.className = 'category-title';
  categoryTitle2.innerText = 'Prompt:';
  container.appendChild(categoryTitle2);

  let inlineDiv: HTMLElement = document.createElement('div');
  inlineDiv.className = 'inline';
  inlineDiv.id = 'inlineContent';
  container.appendChild(inlineDiv);

  // Initial Content Based on Default Selection
  updateContentBasedOnSelection(chip.selectedChips as number);
}

function updateContentBasedOnSelection(selectedChipIndex: number) {
  let inlineDiv: HTMLElement = document.getElementById('inlineContent') as HTMLElement;
  inlineDiv.innerHTML = '';
  if (selectedChipIndex === 0) {
    let yearInput: HTMLElement = document.createElement('input');
    yearInput.id = 'yearInput';
      let textSpan: HTMLElement = document.createElement('span');
      textSpan.id = 'contentText';
      textSpan.className = 'dropdown-size';
      textSpan.innerHTML = 'Provide future data points up to the year ';
      textSpan.appendChild(yearInput);
      textSpan.innerHTML += ' along with the existing data.';
      inlineDiv.appendChild(textSpan);
      let yearDropdown: DropDownList = new DropDownList({
          placeholder: 'Select a Year',
          width: '80px',
          popupHeight: '200px',
          popupWidth: '140px',
          index: 0,
          dataSource: dropdownData
      });
      yearDropdown.appendTo('#yearInput');
  } else if (selectedChipIndex === 1) {
      let textSpan: HTMLElement = document.createElement('span');
      textSpan.id = 'contentText';
      textSpan.innerHTML = 'Suggest the best way to aggregate and view provided fields ';
      let fieldsInput: HTMLElement = document.createElement('input');
      fieldsInput.id = 'fieldsInput';
      textSpan.appendChild(fieldsInput);
      textSpan.innerHTML += ' in ';
      let aggregateInput: HTMLElement = document.createElement('input');
      aggregateInput.id = 'aggregateInput';
      textSpan.appendChild(aggregateInput);
      textSpan.innerHTML += ' aggregate type.';
      inlineDiv.appendChild(textSpan);
      let fieldsMultiSelect: MultiSelect = new MultiSelect({
          placeholder: 'Select Fields',
          width: '150px',
          popupWidth: '180px',
          allowFiltering: true,
          dataSource: ['Country', 'Products', 'Product_Categories', 'Quarter', 'Year', 'Sold', 'Amount', 'In_Stock'], // Sample data
          mode: 'CheckBox',
          value: ['Year', 'Product_Categories', 'Sold']
      });
      fieldsMultiSelect.appendTo('#fieldsInput');
      let aggregateDropdown: DropDownList = new DropDownList({
          placeholder: 'Select aggregation type',
          width: '100px',
          popupHeight: '200px',
          popupWidth: '140px',
          index: 0,
          dataSource: ['Sum', 'Count', 'Product', 'Average', 'Min']
      });
      aggregateDropdown.appendTo('#aggregateInput');
  } else if (selectedChipIndex === 2) {
      let textSpan: HTMLElement = document.createElement('span');
      textSpan.id = 'contentText';
      textSpan.className = 'dropdown-size';
      textSpan.innerHTML = 'Filter the Products field based on ';
      let filterInput: HTMLInputElement = document.createElement('input');
      filterInput.id = 'filterInput';
      textSpan.appendChild(filterInput);
      inlineDiv.appendChild(textSpan);
      let filterTextBox: TextBox = new TextBox({
          placeholder: 'Enter filter category',
          cssClass: 'product-class',
          value: 'Bikes',
          width: '100%'
      });
      filterTextBox.appendTo('#filterInput');
  }
}

function onChipSelectionChange() {
  updateContentBasedOnSelection(chip.selectedChips as number);
}

function toolbarClicked() {
  dialog.show();
}

function onSubmit() {
  dialog.hide();
  createSpinner({
    target: document.querySelector('.e-grid .e-content') as HTMLElement
  });
  showSpinner(document.querySelector('.e-grid .e-content') as HTMLElement);
  if (chip.selectedChips === 0) {
    let year = (document.getElementById('yearInput') as any).value;
    description = `Provide future data points up to the year ${year} along with the existing data from the provided data source.`;
  } else if (chip.selectedChips === 1) {
    let selectedFields = (document.getElementById('fieldsInput') as any).getAttribute("data-initial-value");
    let aggregationValue = (document.getElementById('aggregateInput') as any).value;
    description = `Suggest the best way to aggregate and view provided fields(${selectedFields}) using the provided data source. Use only these fields (${selectedFields}) to frame the rows, columns, and values, ensuring all the provided fields are included in the report and the same field should not be bind twice in different property of reports. **Ensure that the "type" property of the values fields holds the aggregation type as ${aggregationValue}. And the rows and values fields should not be empty in the report`;
  } else if (chip.selectedChips === 2) {
    let filterText: string = document.querySelector('#filterInput') as HTMLInputElement ? (document.querySelector('#filterInput') as HTMLInputElement).value as string : '';
    description = `Filter the Products field based on ${filterText} and return the filtersettings with corresponding items from the Products field `;
  }
  let input: string = frameContent();
  let aiOutput: any = getAzureChatAIRequest({ messages: [{ role: 'user', content: input }] });
  aiOutput.then((result: any) => {
    let cleanedJsonData: string = result.replace(/^```json\n|```\n?$/g, '');
    pivotObj.dataSourceSettings = JSON.parse(cleanedJsonData);
    hideSpinner(document.querySelector('.e-grid .e-content') as HTMLElement);
  });
}

function frameContent(): string {
  let filter: string = "Include, Exclude";
  let labelType: string = "Label, Number";
  let operators: string = `'Equals', 'DoesNotEquals',
    'BeginWith',
    'DoesNotBeginWith',
    'EndsWith',
    'DoesNotEndsWith',
    'Contains',
    'DoesNotContains',
    'GreaterThan',
    'GreaterThanOrEqualTo',
    'LessThan',
    'LessThanOrEqualTo',
    'Before',
    'BeforeOrEqualTo',
    'After',
    'AfterOrEqualTo',
    'Between',
    'NotBetween'`;
  let summary: string = `'Sum',
    'Product'
    'Count',
    'DistinctCount',
    'Median',
    'Min',
    'Max',
    'Avg',
    'Index',
    'PercentageOfGrandTotal',
    'PercentageOfColumnTotal',
    'PercentageOfRowTotal',
    'PercentageOfParentRowTotal',
    'PercentageOfParentColumnTotal',
    'PercentageOfParentTotal',
    'RunningTotals',
    'PopulationStDev',
    'SampleStDev',
    'PopulationVar',
    'SampleVar',
    'DifferenceFrom',
    'PercentageOfDifferenceFrom'`;
    let filterQuery: string = `The filterSettings property holds the filter settings. In this we have two types, member filtering and label filtering. The MemberFiltering has a Type property that is an values corresponding to ${filter} +
    and the MemberFiltering includes the items property that is an array of objects which contains the members of the fields to be included or excluded with the name property. +
    and the LabelFiltering has a Type property that is an values corresponding to ${labelType} +
    and the LabelFiltering property has a Condition property that is an values corresponding to ${operators}. +
    and the LabelFiltering property has a Value1 and Value2 property that depends based on the condition property. +
    Filters should not be applied to fields bound in Values and the same field should not be added to both label filters and member filters.+
    This filterSettings property is an array of objects that contains the filter settings with name and items property for the fields in the pivot table.For example: [{ name: 'Country', type: 'Include', items: ['USA', 'UK' ] }].+`;
  let filterItem: string = document.querySelector('#filterInput') as HTMLInputElement ? (document.querySelector('#filterInput') as HTMLInputElement).value as string : '';
  let pivotQuery: string = `The values property has a type property, which is an enum with values corresponding to ${summary}.`;
  let stringInput: string = `Given the following dataSource and the datasourcesettings(rows, columns and values) are bounded in the pivot table ${JSON.stringify(pivotData)} , ${JSON.stringify(dataSourceSettings)} respectively. 
  Return the newly prepared dataSourceSettings based on following user query: ${description} and the data source shouldn't change if the query contains a future data points and the reframed data source should contains all the fields(key) with its corresponding values(please refer the first object of the provided data source for the keys), And the items in the filters should be just an array of values, not objects. And the value of the items should be ${filterItem}.
  Generate an output in JSON format only and Should not include any additional information or content in response.
  Note: ${pivotQuery},
  ${filterQuery}`;
  return stringInput;
}

import { Directive, ElementRef, Injector, Input } from '@angular/core';
import { NgbDatepicker, NgbDateStruct, NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { fromEvent } from 'rxjs';
import { THEME_SHARED_OPTIONS } from '../tokens/options.token';

@Directive({
  // tslint:disable-next-line
  selector: 'ngb-datepicker, input[ngbDatepicker]',
})
export class NgbDatepickerDefaultDirective {
  @Input() minDate: NgbDateStruct;
  @Input() maxDate: NgbDateStruct;
  @Input() navigation: 'select' | 'arrows' | 'none' = 'select';

  datepicker: NgbDatepicker | NgbInputDatepicker;

  constructor(private elRef: ElementRef<HTMLElement>, private injector: Injector) {
    const {
      ngbDatepickerOptions: { minDate, maxDate },
    } = this.injector.get(THEME_SHARED_OPTIONS);

    this.datepicker =
      this.injector.get(NgbInputDatepicker, null) || this.injector.get(NgbDatepicker, null);
    if (!this.datepicker) return;

    this.datepicker.minDate = this.minDate || minDate;
    this.datepicker.maxDate = this.maxDate || maxDate;
    this.datepicker.navigation = this.navigation;

    if (this.datepicker.navigation !== 'select') return;

    setTimeout(() => {
      if (this.datepicker instanceof NgbDatepicker) {
        this.configureSelectStyle(this.elRef.nativeElement);
      }

      if (this.datepicker instanceof NgbInputDatepicker) {
        this.datepicker.toggle = () => {
          if ((this.datepicker as NgbInputDatepicker).isOpen()) {
            (this.datepicker as NgbInputDatepicker).close();
          } else {
            (this.datepicker as NgbInputDatepicker).open();
            this.configureSelectStyle(this.elRef.nativeElement.nextElementSibling as HTMLElement);
          }
        };
      }
    }, 0);
  }

  configureSelectStyle = (parentElement: HTMLElement) => {
    const selectElements = parentElement.querySelectorAll('select');

    selectElements.forEach((element, i) => {
      fromEvent(element, 'focus').subscribe(() => {
        element.size = 10;

        const { offsetTop } = element.querySelector(
          `[value="${element.value}"]`,
        ) as HTMLOptionElement;
        element.scrollTo({
          top: offsetTop - 10,
        });
      });

      fromEvent(element, 'blur').subscribe(() => {
        element.size = 1;
      });

      fromEvent(element, 'change').subscribe(() => {
        element.size = 1;
        setTimeout(() => element.blur(), 0);
      });
    });
  };
}

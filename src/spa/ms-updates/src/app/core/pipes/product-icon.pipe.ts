import { Pipe, PipeTransform } from '@angular/core';

const PRODUCT_ICONS: Record<string, string> = {
  azure: 'assets/icons/azure.svg',
  fabric: 'assets/icons/fabric.svg',
  github: 'assets/icons/github.svg',
  foundry: 'assets/icons/foundry.svg',
  copilot365: 'assets/icons/copilot-color.svg',
  'microsoft foundry': 'assets/icons/foundry.svg',
  'microsoft fabric': 'assets/icons/fabric.svg',
  'microsoft copilot 365': 'assets/icons/copilot-color.svg'
};

@Pipe({
  name: 'productIcon',
})
export class ProductIconPipe implements PipeTransform {
  transform(productName: string | null | undefined): string {
    const normalizedName = productName?.trim().toLowerCase() ?? '';

    return PRODUCT_ICONS[normalizedName] ?? 'ms-icon.svg';
  }
}